# Motor de requisitos regulatorios RRNA v1

Sprint 3 añade una capa documental independiente para determinar qué requisito regulatorio podría aplicar a una operación y qué evidencia clasificada está disponible. Es un apoyo conservador: **no declara cumplimiento o incumplimiento, no produce `Finding`, no produce `NON_COMPLIANT` y no modifica `AuditResult` LEGAL**.

## Modelo y versionado

`RegulatoryRequirement` se identifica por `Code + Version` y usa los estados `DRAFT`, `VERIFIED` y `RETIRED`. Una versión `VERIFIED` es inmutable desde la API; los cambios se realizan creando otra versión `DRAFT`. La evaluación selecciona, por código, la versión `VERIFIED` más alta que esté vigente en `TradeOperation.OperationDate`. Cuando no existe una verificada vigente puede mostrar la DRAFT más alta, pero su resultado se fuerza a `MANUAL_REVIEW`.

La vigencia se calcula exclusivamente con la fecha de la operación y los intervalos `EffectiveFrom`/`EffectiveTo`, nunca con la fecha actual. El seed incluye sólo escenarios sintéticos `DEMO-RRNA-*` en estado DRAFT; no contiene obligaciones, fuentes ni URLs regulatorias reales.

## Condiciones seguras

Los campos iniciales son `TARIFF_FRACTION`, `NICO`, `OPERATION_TYPE`, `CUSTOMS_REGIME`, `ORIGIN_COUNTRY`, `DESTINATION_COUNTRY`, `CUSTOMS_OFFICE`, `DESCRIPTION`, `QUANTITY`, `UNIT`, `COMMERCIAL_VALUE` y `CURRENCY`.

- Las condiciones dentro de un mismo `ConditionGroup` se unen con AND.
- Los grupos alternativos se unen con OR.
- Los operadores permitidos son `EQUALS`, `NOT_EQUALS`, `IN`, `NOT_IN`, `PREFIX`, `NUMERIC_RANGE`, `PRESENT`, `NOT_PRESENT` y `CONTAINS_NORMALIZED`.
- Fracción arancelaria y NICO se normalizan por separado; se eliminan separadores pero nunca se mezcla un campo con el otro.
- Países usan una equivalencia ISO controlada y local. Una denominación ambigua produce datos insuficientes.
- Una coincidencia parcial de descripción con `CONTAINS_NORMALIZED` requiere revisión manual; no produce por sí sola una aplicación definitiva.

Los resultados automáticos son `APPLIES`, `DOES_NOT_APPLY`, `INSUFFICIENT_DATA` y `MANUAL_REVIEW`. El snapshot conserva valores originales, normalizados, condiciones, versión y evidencia observada.

## Evidencia

`RegulatoryEvidenceRequirement` puede definir evidencia primaria, de soporte o alternativa. Los requisitos sin grupo alternativo deben satisfacerse individualmente; dentro de un mismo grupo alternativo basta cualquier miembro. Los estados son `COMPLETE`, `PARTIAL`, `MISSING_EVIDENCE`, `INSUFFICIENT_DATA`, `MANUAL_REVIEW` y `NOT_REQUIRED`.

Los tipos documentales añadidos son `NOM_EVIDENCE`, `PERMISO_PREVIO`, `AVISO_AUTOMATICO`, `COFEPRIS_DOCUMENT`, `SEMARNAT_DOCUMENT`, `SEDENA_DOCUMENT`, `FITOSANITARIO_CERTIFICATE` y `RRNA_OTHER`. El clasificador sólo propone estos tipos usando señales transparentes de nombre o texto; una clasificación no acredita autenticidad, validez o suficiencia jurídica.

## Revisión humana y trazabilidad

Cada ejecución agrega `RegulatoryRequirementEvaluation` y sus filas de evidencia; nunca modifica ejecuciones anteriores. La revisión humana conserva siempre `AutomatedOutcome` y agrega uno de los siguientes resultados: `CONFIRMED_APPLIES`, `CONFIRMED_DOES_NOT_APPLY`, `EVIDENCE_ACCEPTED`, `EVIDENCE_REJECTED` o `NEEDS_MORE_INFORMATION`, además de nota, usuario y fecha.

La bitácora registra creación, edición, nueva versión, inicio/fin de evaluación y revisión manual. Si RRNA falla durante la auditoría documental, la API responde con `REGULATORY_CONTROLLED_FAILURE`; valoración, contribuciones y LEGAL mantienen su independencia.

## API autenticada

- `GET /api/operations/{id}/regulatory-requirements`
- `POST /api/operations/{id}/regulatory-requirements/evaluate`
- `GET /api/operations/{id}/regulatory-requirements/history`
- `POST /api/operations/{id}/regulatory-requirements/{evaluationId}/review`
- `GET /api/regulatory-requirements`
- `GET /api/regulatory-requirements/{id}`
- `POST /api/regulatory-requirements`
- `PUT /api/regulatory-requirements/{id}`
- `POST /api/regulatory-requirements/{id}/version`

## Limitaciones deliberadas

No hay integración con fuentes oficiales externas, scraping, OCR, IA generativa ni actualización automática de regulaciones. No se incluyen datos reales de clientes ni reglas regulatorias reales. Antes de usar versiones `VERIFIED` en producción se requiere gobierno de datos y validación especializada de fuente, vigencia, alcance, condiciones y evidencia.

