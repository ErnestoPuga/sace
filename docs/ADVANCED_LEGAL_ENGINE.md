# Motor avanzado de decisión LEGAL V2

Sprint 7 incorpora un motor jurídico separado de los controles `DEMO`, `TECH`, cruces, valoración, contribuciones, RRNA, IMMEX y riesgo. El resultado técnico alimenta evidencia; nunca se convierte por sí solo en incumplimiento. La única dirección permitida es `LEGAL → riesgo` para priorización operativa.

> Los resultados de SACE constituyen apoyo para revisión de cumplimiento y no sustituyen la determinación de la autoridad competente ni el análisis jurídico profesional cuando éste sea necesario.

## Decisión y vigencia

`ILegalAuditEngineV2` selecciona una sola versión `VERIFIED` por código usando la fecha de la operación. La fuente y su versión también deben estar `VERIFIED`. `PublicationDate` registra cuándo se publicó una disposición y `EffectiveFrom` cuándo puede aplicarse: no son fechas equivalentes.

La reforma informada como publicada el 23/10/2026, con vigencia desde el 01/01/2027 y alcance de referencia en los artículos 144, 151, 153, 154, 177 y 185 de la Ley Aduanera, se registra como metadata `STAGED`. No participa en operaciones de 2026 ni en decisiones productivas hasta ser cotejada y verificada contra la publicación oficial.

El motor produce exactamente uno de estos resultados por regla:

- `COMPLIANT`
- `NON_COMPLIANT`
- `NOT_APPLICABLE`
- `INSUFFICIENT_EVIDENCE`
- `MANUAL_REVIEW`

Sólo `NON_COMPLIANT` crea un hallazgo. La ausencia de evidencia no presume incumplimiento.

## Modelo y trazabilidad

- `NormativeRule` conserva código, versión, vigencia, estado y nivel de automatización.
- `LegalRuleCondition` configura grupos de aplicabilidad y decisión.
- `LegalEvidenceRequirement` y `LegalEvidenceMapping` definen prueba requerida y su procedencia.
- `LegalRuleSourceCitation` exige citas oficiales antes de verificar una regla.
- `AuditResult` conserva snapshot de regla, fuente, cita, vigencia y explicación.
- `LegalEvaluationEvidence` conserva el origen técnico y valor usados.
- `LegalReviewDecision` registra confirmación, override o solicitud de información sin sobrescribir el resultado automático.
- `Finding` enlaza resultado original, último resultado, corrección, confirmación o descarte.

Las reglas verificadas son inmutables. Todo cambio requiere una nueva versión `DRAFT`, que pasa por verificación antes de ser elegible.

## Evidencia

El resolver soporta datos de operación, documentos, campos documentales, cruces, valoración, contribuciones, RRNA e IMMEX. Cada mapping es explícito y el resultado conserva el identificador de la entidad origen. Riesgo no es una fuente de evidencia LEGAL.

## Revisión y corrección

La revisión humana requiere razón y conserva el resultado automático. Un override a `NON_COMPLIANT` puede crear un hallazgo controlado; un override a `COMPLIANT` descarta el hallazgo relacionado con trazabilidad. Los hallazgos pasan por estados `OPEN`, `CONFIRMED`, `CORRECTED`, `DISMISSED` y `CLOSED`; una re-evaluación crea un resultado nuevo y no modifica snapshots anteriores.

## API

- `POST /api/operations/{id}/legal/evaluate`
- `GET /api/operations/{id}/legal`
- `GET /api/operations/{id}/legal/history`
- `GET /api/operations/{operationId}/legal/{resultId}`
- `POST /api/legal/results/{id}/review`
- `GET /api/legal/findings` y `GET /api/legal/findings/{id}`
- `POST /api/legal/findings/{id}/review|correction|close`
- `GET|POST /api/legal-rules`
- `GET|PUT /api/legal-rules/{id}`
- `POST /api/legal-rules/{id}/version|verify`

Todos requieren JWT. Los listados aceptan filtros y paginación.

## Lotes y migración

La etapa `LEGAL` del pipeline por lotes usa V2 antes del scoring. La migración incremental `AdvancedLegalDecisionEngineV1` agrega tablas, columnas, índices y relaciones; no reemplaza ni reescribe migraciones históricas.

## Límites explícitos

SACE no calcula sanciones, no determina fraude o responsabilidad penal, no hace clasificación arancelaria autónoma, no consulta SAT/VUCEM en línea y no sustituye autoridad ni asesoría jurídica.
