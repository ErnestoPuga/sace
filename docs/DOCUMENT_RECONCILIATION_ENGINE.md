# Document Reconciliation Engine V1

## Purpose

Document Reconciliation agrega una vista integral sobre el motor CROSS. Compara los campos estructurados y las partidas soportadas entre parejas documentales configuradas, sin comparar bytes, layout ni texto completo. El resultado es técnico y nunca equivale por sí mismo a cumplimiento o incumplimiento jurídico.

## Atomic CROSS vs reconciliation

- `CrossValidationRule` sigue siendo la comparación atómica y versionada de un campo.
- `DocumentReconciliationProfile` agrupa campos semánticamente comparables entre dos tipos documentales.
- `DocumentReconciliationResult` conserva el resumen histórico de una pareja concreta.
- Cuando existe un `CrossValidationResult` para la misma regla y pareja, la reconciliación lo referencia y reutiliza. No se ejecuta un segundo algoritmo para ese dato.

Risk continúa consumiendo resultados CROSS atómicos. No consume el resumen de reconciliación, por lo que una diferencia no se penaliza dos veces.

## Initial profiles

- `RECON-INV-COVE-V1`: factura comercial ↔ COVE.
- `RECON-INV-PED-V1`: factura comercial ↔ pedimento.
- `RECON-PL-PED-V1`: packing list ↔ pedimento.
- `RECON-PL-TR-V1`: packing list ↔ documento de transporte.

Los perfiles tienen estado `DRAFT`, `ACTIVE` o `RETIRED`, versión, vigencia y una lista ordenada de campos. Las secciones disponibles son identificación, fechas, mercancía, cantidades, valores, logística, términos comerciales, referencias, partidas y otros.

## Fields, outcomes and coverage

Cada detalle conserva campo izquierdo/derecho, raw, normalizado, método de extracción, origen de evidencia, tolerancia, diferencia, explicación y regla CROSS reutilizada. Los outcomes integrales son:

- `MATCH`: toda la evidencia evaluable coincide.
- `WITH_DIFFERENCES`: existe al menos una diferencia o partida sin pareja.
- `INSUFFICIENT_EVIDENCE`: falta evidencia estructurada y no existe diferencia acreditada.
- `MANUAL_REVIEW`: el pairing documental o de partidas es ambiguo.
- `NOT_APPLICABLE`: no existen comparaciones aplicables.

`CoveragePercent` es campos aplicables con evidencia suficiente / campos aplicables esperados. Incluye coincidencias y diferencias como campos evaluados. No representa compliance.

## Document pairing

Una pareja única se usa directamente. Cuando existen múltiples documentos, el servicio utiliza intersección exacta normalizada de `DocumentReference`, `INVOICE_NUMBER`, `COVE_NUMBER`, `DOCUMENT_NUMBER` y metadata documental. No usa `First()`, orden posicional ni combinación global de cantidades. Una referencia con múltiples candidatos produce `MANUAL_REVIEW`.

## Line items

`DocumentLineItem` representa partidas determinísticamente identificadas por marcadores `ITEM`, `LINE ITEM` o `PARTIDA`. V1 soporta número de partida, part number, SKU, descripción, cantidad, unidad, valor unitario, total, moneda, fracción, NICO, pesos y país de origen.

La reextracción reemplaza únicamente partidas automáticas del documento actual. No se inventan partidas si no existe una estructura reconocible. El límite `DocumentReconciliation:MaxLineItemsPerDocument` evita crecimiento ilimitado; el valor inicial es 500.

## Line item matching

`IDocumentLineItemMatcher` aplica claves fuertes en este orden:

1. PartNumber/SKU exacto normalizado.
2. Item/reference exacta.
3. Descripción normalizada + cantidad + unidad.

No existe fuzzy matching, IA, embeddings ni selección arbitraria. Múltiples candidatas producen `AMBIGUOUS`; partidas sin candidata quedan `UNMATCHED_LEFT` o `UNMATCHED_RIGHT`. Una vez vinculadas, los campos configurados se comparan independientemente.

## Evidence priority and fallback

La prioridad es `MANUAL > XML > PDF_TEXT > FILENAME > EXISTING_METADATA`. La evidencia extraída del documento siempre precede al fallback. `EXISTING_METADATA` permanece visible en el resultado y nunca se presenta como `PDF_TEXT`.

## Snapshots and history

Cada ejecución crea nuevos resultados. `SnapshotJson` conserva perfil, documentos, raw/normalizados, métodos, outcomes, diferencias, tolerancias, partidas, claves de matching y fecha. Reextraer documentos no reescribe resultados anteriores.

## API

Todos los endpoints requieren JWT:

- `GET /api/operations/{id}/reconciliations?page=1&pageSize=20`
- `POST /api/operations/{id}/reconciliations/run`
- `GET /api/operations/{id}/reconciliations/{reconciliationId}`
- `GET /api/operations/{id}/document-line-items?page=1&pageSize=50`

`POST /cross-validations/run` ejecuta reglas atómicas y luego perfiles. Los endpoints CROSS existentes se conservan.

## UI

La vista `/operations/{id}/reconciliations` muestra primero tarjetas integrales, cobertura, matches, diferencias, evidencia insuficiente y partidas vinculadas. El detalle agrupa por sección y muestra evidencia y explicación. Las reglas atómicas siguen accesibles en el expediente.

## Performance and security

- Los listados son paginados y respetan `Pagination:MaxPageSize`.
- El matching reduce candidatos por claves fuertes y evita un producto cartesiano indiscriminado.
- Las lecturas usan `AsNoTracking` y carga agrupada para evitar N+1.
- Se indexan documento/secuencia, documento/part number, perfil/versión y operación/fecha.
- No se expone texto completo del PDF, JWT, contraseñas ni secretos.
- Auditoría: `DOCUMENT_RECONCILIATION_STARTED`, `COMPLETED` y `MANUAL_REVIEW`.

## LEGAL separation

Reconciliation no crea `AuditResult` LEGAL ni `Finding`. LEGAL V2 sólo puede consumir evidencia técnica mediante una regla jurídica explícita y verificada. `WITH_DIFFERENCES` no significa `NON_COMPLIANT`.

## Limitations

- No OCR, IA generativa, embeddings, fuzzy ni semantic matching.
- No interpreta tablas arbitrarias ni garantiza extracción de todos los PDFs.
- No compara texto completo, bytes ni layout.
- No todos los campos entre documentos son semánticamente equivalentes.
- Una estructura ambigua requiere revisión humana.
- Los perfiles soportados inicialmente se limitan a las cuatro parejas documentales indicadas.
- Coverage no equivale a compliance y una diferencia documental no equivale a incumplimiento LEGAL.
