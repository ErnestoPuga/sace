# Customs Valuation Engine

## Objetivo

El motor de valoración aduanera de SACE 2.0 Sprint 2 produce una conciliación técnica, reproducible y trazable entre el valor de transacción, los incrementables documentados y el valor en aduana declarado. No determina cumplimiento jurídico, sanciones ni créditos fiscales.

## Arquitectura

El módulo se mantiene dentro del monolito modular y se expone mediante `ICustomsValuationEngine`, `IMonetaryConversionService` e `IContributionReconciliationEngine`. Cada ejecución genera un registro nuevo e inmutable con versión, explicación, fórmula, snapshot y referencias a evidencia.

```text
Document analysis → DocumentField → CrossValidation
                                      ↓
                             Customs Valuation
                                      ↓
                         Contribution Reconciliation

LEGAL permanece como motor independiente.
```

## Relación con DocumentField

Los importes, monedas, tasas, referencias y datos de pago proceden de `DocumentField`. Se conserva `RawValue`, el valor normalizado, método de extracción, confianza, documento y localizador. Cuando se usa metadata de `TradeOperation`, se identifica expresamente como `EXISTING_METADATA` en la explicación o fuente.

## Relación con CrossValidation

Las once reglas nuevas comparan evidencia documental de flete, seguro, incrementables y valoración. `CrossValidationResult` sirve como señal técnica independiente; no se convierte en un componente monetario adicional y no modifica resultados LEGAL.

## Diferencia con LEGAL

`ValuationOutcome.MISMATCH`, `ContributionOutcome.MISMATCH` y `CrossValidationOutcome.MISMATCH` describen diferencias técnicas. No producen `LegalOutcome.NON_COMPLIANT`, no crean `Finding` y no alteran un `AuditResult` LEGAL existente.

## Transaction Value

La prioridad V1 es: Manifestación de Valor/MVE, factura comercial o COVE, y finalmente metadata existente de la operación. Si no hay base confiable, el resultado es `INSUFFICIENT_EVIDENCE`; nunca se sustituye por cero.

## Incrementables

V1 representa por separado `FREIGHT`, `INSURANCE`, `PACKAGING` y `OTHER_INCREMENTABLE`. Cada componente sumable tiene importe original, moneda, conversión y evidencia. Un gasto ambiguo se conserva como `UNCLASSIFIED_INCREMENTABLE`, con tratamiento informativo y revisión manual.

## CustomsValuationComponent

Representa un concepto económico único dentro de una versión de cálculo. `EconomicKey` y el índice único por cálculo impiden duplicar el mismo componente persistido. `Treatment` determina si se suma o si sólo se presenta como información.

## ValuationComponentEvidence

Relaciona un componente con sus documentos y campos. Los roles son `PRIMARY`, `SUPPORTING`, `PAYMENT` y `DECLARED_VALUE`. Varias evidencias pueden respaldar un solo componente sin repetir su importe.

## Prevención de doble conteo

Factura de flete, AWB/documento de transporte, pago y valor declarado se agrupan como evidencia del mismo `FREIGHT`. El importe se toma una sola vez de la fuente primaria priorizada. Documentos con el mismo SHA-256 se consolidan por hash dentro de la evidencia. Importes primarios distintos se escalan a revisión manual en lugar de sumarse arbitrariamente.

## Monedas

Toda la cadena usa `decimal`. Si origen y destino coinciden, el factor es 1. Una conversión entre monedas requiere código de moneda, tasa positiva y fuente identificada. Moneda faltante, tasa cero/negativa o fuente ausente producen un estado controlado.

## Tipo de cambio

La prioridad V1 es el campo `EXCHANGE_RATE` del pedimento, el campo equivalente de MV/MVE y después `TradeOperation.ExchangeRate`. Se persisten `ExchangeRate`, `ExchangeRateSource`, moneda original y monto convertido. No se consultan fuentes externas ni se inventan tasas.

## Fórmula

```text
valor de transacción convertido
+ flete convertido
+ seguro convertido
+ embalaje convertido
+ otros incrementables convertidos
= valor en aduana calculado
```

La fórmula materializada por el backend se persiste en `Formula`. El frontend sólo la presenta; no recalcula.

## Tolerancias

`Valuation:AbsoluteTolerance` y `Valuation:PercentTolerance` son configuración explícita y quedan copiadas en cada cálculo. La conciliación se considera coincidente si satisface al menos una tolerancia. `Contribution:AbsoluteTolerance` controla la comparación declarado/pagado.

## Outcomes

- `MATCH`: cálculo y declaración coinciden dentro de tolerancia.
- `MISMATCH`: existe una diferencia técnica calculable.
- `INSUFFICIENT_EVIDENCE`: faltan base, declaración, moneda, tasa o evidencia necesaria.
- `MANUAL_REVIEW`: existe ambigüedad económica o evidencia primaria contradictoria.
- `NOT_APPLICABLE`: reservado para una determinación explícita de no aplicabilidad.

## MV/MVE

MV/MVE puede aportar valor de transacción, flete, seguro y tipo de cambio. Se trata como evidencia, no como fuente infalible. Sus datos pueden participar en cruces con pedimento y requieren la misma trazabilidad que cualquier documento.

## Pedimento

El extractor reconoce `COMMERCIAL_VALUE`, `DECLARED_FREIGHT`, `DECLARED_INSURANCE`, `DECLARED_PACKAGING`, `DECLARED_OTHER_INCREMENTABLES`, `DECLARED_CUSTOMS_VALUE` y `EXCHANGE_RATE` cuando existe evidencia legible. Los códigos anteriores permanecen compatibles.

## Contribution Reconciliation

La conciliación vincula líneas declaradas con pagos por código de contribución. Compara declarado, calculado disponible y pagado. El cálculo base × tasa está deshabilitado por defecto y sólo puede activarse mediante configuración; una tasa ausente nunca se supone.

## ContributionLine

Cada línea conserva código, descripción, base, tasa e importe raw, además de sus valores normalizados, diferencias, moneda, documentos de origen/pago y explicación. No genera efectos LEGAL.

## Códigos desconocidos

Un código como `XYZ` se conserva en `RawContributionCode` y `ContributionCode`. No se elimina, reinterpreta ni provoca excepción; permanece visible con evidencia insuficiente cuando no puede conciliarse.

## Historial

Cada recálculo incrementa `CalculationVersion` o `Version` y crea un registro nuevo. La API ofrece el último cálculo y el historial de valoración; las versiones anteriores no se mutan.

## API

- `GET /api/operations/{id}/valuation`
- `GET /api/operations/{id}/valuation/history`
- `POST /api/operations/{id}/valuation/calculate`
- `GET /api/operations/{id}/contributions`
- `POST /api/operations/{id}/contributions/reconcile`

Todas las rutas requieren JWT. Operaciones inexistentes y errores se devuelven con el contrato estructurado `ApiError`, sin stack trace.

## Frontend

La pestaña **Valoración aduanera** del detalle muestra totales persistidos, resultado, fórmula, tasa y fuente, tolerancias, componentes con evidencia agrupada y la tabla de contribuciones. Los valores desconocidos se representan con `—`, no con cero.

## System Audit Log

Se registran `VALUATION_STARTED`, `VALUATION_COMPLETED`, `VALUATION_RECALCULATED`, `CONTRIBUTION_RECONCILIATION_STARTED` y `CONTRIBUTION_RECONCILIATION_COMPLETED`. El usuario sólo se referencia cuando existe, evitando violaciones de llave foránea.

## Seguridad

El módulo reutiliza autenticación JWT, validación de operación, middleware de errores y límites documentales existentes. Snapshots y logs no almacenan tokens, contraseñas ni rutas físicas. Los endpoints GET no disparan cálculos ni mutaciones.

## Limitaciones

- La extracción PDF sólo procesa texto embebido; no hay OCR.
- V1 selecciona una fuente primaria priorizada por tipo de componente; múltiples gastos legítimos de la misma categoría requieren revisión y evolución del modelo.
- No se valida oficialmente ante SAT, ANAM o VUCEM.
- No se determinan multas, sanciones ni consecuencias legales.
- No se obtienen tasas ni tipos de cambio desde Internet.

## Roadmap

Fuera de Sprint 2 quedan: múltiples partidas económicas por categoría con claves documentales robustas, catálogos oficiales versionados, fuentes autorizadas de tipo de cambio, OCR, conciliación de pagos parciales y validación especializada antes de uso productivo.
