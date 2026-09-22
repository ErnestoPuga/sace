# Motor Normativo Real v1

## Alcance

El motor LEGAL de SACE evalúa un conjunto acotado y trazable de reglas sobre la información estructurada y la evidencia disponible en cada expediente. No es una revisión integral de comercio exterior y no sustituye el análisis de un especialista.

Los grupos permanecen separados:

- `LEGAL-*`: reglas con fundamento, vigencia, versión, evidencia y resultado jurídico automatizable.
- `TECH-*`: integridad técnica de archivos y referencias; no participa en el cumplimiento legal.
- `DEMO-*`: compatibilidad con el flujo demostrativo original; no participa en el cumplimiento legal.

## Corpus base

La semilla registra referencias descriptivas para:

- Ley Aduanera, con fecha de publicación/reforma capturable por versión.
- Reglas Generales de Comercio Exterior 2026 y su regla 3.1.8 y 1.9.16.
- Anexo 22 de las RGCE 2026, incluida una segunda versión efectiva desde el 20 de mayo de 2026 para demostrar la selección temporal.
- Tratado o acuerdo comercial declarado para el análisis manual de origen preferencial.

`OfficialUrl` es nullable y queda disponible para administración manual. La semilla no inventa direcciones oficiales. Antes de uso productivo, un especialista debe confirmar fuente, publicación, vigencia, alcance y configuración de cada versión.

## Selección de versión

La fecha de evaluación v1 es `TradeOperation.OperationDate` y se conserva en `Audit.EvaluationDate`, con base `OPERATION_DATE`. Para cada código se consideran solamente versiones activas donde:

```text
EffectiveFrom <= EvaluationDate
y (EffectiveTo es nulo o EvaluationDate <= EffectiveTo)
```

Si varias versiones son válidas, se usa la de mayor número. El resultado conserva `NormativeRuleId` y `NormativeRuleVersion`; una modificación posterior del catálogo no altera el historial.

## Resultados

- `COMPLIANT`: la evidencia disponible permite concluir conformidad con el criterio automatizado.
- `NON_COMPLIANT`: existe una discrepancia comprobable; puede crear hallazgo.
- `NOT_APPLICABLE`: el supuesto de la regla no corresponde a la operación.
- `INSUFFICIENT_EVIDENCE`: faltan datos o documentos para decidir; no crea hallazgo por sí solo.
- `MANUAL_REVIEW`: la determinación requiere juicio humano; no crea hallazgo por sí solo.

El dictamen humano se guarda en `ManualOutcome`, `ReviewComment`, `ReviewerUserId` y `ReviewedAt`. Nunca reemplaza `Outcome`, por lo que se conserva la conclusión automática original.

## Métricas

El cumplimiento automatizado usa solamente resultados decidibles:

```text
COMPLIANT / (COMPLIANT + NON_COMPLIANT)
```

Se muestra siempre junto con la cobertura:

```text
(COMPLIANT + NON_COMPLIANT) /
(total incluido - NOT_APPLICABLE)
```

Así, un 100% sobre una fracción pequeña del expediente no se presenta sin exponer los casos pendientes por evidencia o revisión humana.

## Reglas v1

| Código | Fundamento configurado | Automatización |
|---|---|---|
| `LEGAL-LA36-001` | Ley Aduanera, artículo 36 | Semiautomática: presencia y coincidencia de pedimento |
| `LEGAL-LA36A-VAL-001` | Ley Aduanera, artículo 36-A, fracción I, inciso a | Semiautomática: evidencia relativa al valor |
| `LEGAL-RGCE-318-001` | RGCE 2026, regla 3.1.8 | Automática: umbral configurado de USD 300 |
| `LEGAL-RGCE-318-002` | RGCE 2026, regla 3.1.8 | Semiautomática: campos extraíbles de factura |
| `LEGAL-ANEXO22-INV-001` | Anexo 22, bloque factura/documento equivalente | Semiautomática: número, moneda y valor |
| `LEGAL-RGCE-1916-001` | RGCE 2026, regla 1.9.16 | Automática: consistencia COVE |
| `LEGAL-LA36A-TRANSPORT-001` | Ley Aduanera, artículo 36-A, fracción I, inciso b | Semiautomática: evidencia y pertinencia de transporte |
| `LEGAL-LA65-001` | Ley Aduanera, artículo 65 | Semiautomática: incrementables confirmados y cálculo |
| `LEGAL-ORIGIN-001` | Tratado o acuerdo declarado | Manual cuando existe preferencia; no aplicable en otro caso |
| `LEGAL-EDOC-001` | RGCE 2026 | Automática: consistencia e-Document |
| `LEGAL-RRNA-001` | Ley Aduanera y disposiciones sectoriales | Manual: presenta fracción, NICO, descripción, origen, régimen e identificadores |

## Limitaciones jurídicas

- La regla de USD 300 depende de que el valor esté expresado en USD; no convierte monedas automáticamente.
- Los PDF sin extracción estructurada pasan a revisión manual; no se supone su contenido.
- La pertinencia del documento de transporte depende del modo y circunstancias de la operación.
- Los conceptos de valor con calificación pendiente o moneda distinta se remiten a revisión humana.
- La prueba de origen, RRNA, permisos, NOM, cuotas y regulaciones sectoriales no se determinan automáticamente en v1.
- No hay consulta en línea a SAT, ANAM, VUCEM, DOF ni autoridades sectoriales.
- El conjunto de reglas es deliberadamente limitado y debe validarse por especialistas antes de uso productivo.

## Trazabilidad y eventos

Se registran `LEGAL_AUDIT_EXECUTED`, `LEGAL_RULE_EVALUATED`, `MANUAL_REVIEW_COMPLETED` y `NORMATIVE_RULE_VERSION_CHANGED`. Los registros contienen código, versión y resultado, pero no copian archivos, tokens ni contraseñas.

## Migración y API

La migración incremental `NormativeEngineV1` agrega los campos normativos, evidencia, revisión y `CustomsValueAdjustments` sobre la base existente.

- `POST /api/operations/{id}/legal-audit`
- `GET /api/operations/{id}/legal-audits`
- `GET|POST /api/operations/{id}/adjustments`
- `POST /api/audit-results/{id}/manual-review`

## Prueba del caso patrón

1. Crear la importación del 19/07/2026 y capturar pedimento, factura, COVE, valor, moneda, tipo de cambio y valor en aduana declarado.
2. Cargar pedimento, factura, COVE, transporte y documentos con referencias e-Document.
3. Completar metadata estructurada (números, moneda, total y referencias) cuando la extracción automática no la produzca.
4. Registrar incrementables y confirmar su calificación jurídica solamente cuando corresponda.
5. Ejecutar **Auditoría → Ejecutar auditoría legal**.
6. Revisar versión aplicada, cinco contadores, cumplimiento decidible, cobertura, evidencia, valores comparados y cálculo.
7. Registrar dictamen humano en reglas manuales; comprobar que el resultado automático se conserva.

