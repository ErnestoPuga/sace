# Auditoría por lotes y riesgo operativo

SACE 2.0 Sprint 5 incorpora procesamiento por lotes, scoring explicable y una cola de revisión humana. El score sirve para ordenar trabajo operativo: **no es una conclusión de cumplimiento o incumplimiento legal** y no modifica resultados del motor `LEGAL`.

## BatchAudit y snapshot

Al crear un lote, el backend resuelve los filtros y persiste una fila `BatchAuditOperation` por operación. Ese conjunto es el snapshot reproducible: las operaciones creadas después no entran silenciosamente. El máximo se configura mediante `BatchAudit:MaxOperationsPerBatch` (500 por omisión).

`IBatchAuditService` crea el lote, controla progreso, ejecuta, cancela y reintenta. `IBatchOperationPipeline` aísla la ejecución por operación para una futura implementación background. El orden actual es:

1. análisis documental con el motor diario;
2. extracción de campos y cruces (`ICrossValidationEngine` invoca `IDocumentFieldExtractor`);
3. valoración aduanera;
4. conciliación de contribuciones;
5. evaluación RRNA;
6. auditoría LEGAL;
7. cálculo de riesgo.

Cada etapa registra su propio resultado. Una excepción controlada se agrega al error de la operación y no detiene las siguientes. El lote termina `COMPLETED_WITH_ERRORS` si alguna falla. `retry-failed` selecciona sólo filas `FAILED`; la cancelación se observa entre operaciones y conserva resultados terminados.

## Perfil y score

El seed idempotente `RISK-OPS-V1` es un perfil `ACTIVE` de pesos de priorización operativa V1. No representa una metodología jurídica oficial. `RiskFactorDefinition` configura fuente, outcome, peso, máximo, severidad, correlación y explicación. Los topes por categoría viven en el perfil.

El algoritmo:

1. consume los resultados persistidos más recientes de cada motor;
2. excluye `DEMO-*` y RRNA `DRAFT`/no verificada;
3. limita cada factor a `MaxPoints` y evita valores negativos;
4. para una misma `CorrelationKey`, usa `MAX` cuando así lo configura el factor;
5. aplica el tope por categoría;
6. normaliza a `0..100` y deriva `LOW`, `MEDIUM`, `HIGH` o `CRITICAL` con umbrales del perfil.

Cada `OperationRiskScore` conserva versión, perfil, score bruto/normalizado, nivel, explicación, factores y snapshot JSON. Los scores anteriores nunca se sobrescriben. Un factor conserva código, categoría, puntos, fuente y entidad de origen para responder por qué una operación recibió ese score.

## Review Queue

Un score `HIGH` o `CRITICAL` crea o actualiza una revisión abierta. Un índice único filtrado impide más de una revisión abierta por operación. La prioridad manual conserva `OriginalPriority`, `CurrentPriority`, usuario, fecha y motivo; una recalculación no la sobrescribe.

Transiciones admitidas:

- `OPEN → ASSIGNED | IN_REVIEW`
- `ASSIGNED → IN_REVIEW`
- `IN_REVIEW → WAITING_INFORMATION | RESOLVED | DISMISSED`
- `WAITING_INFORMATION → IN_REVIEW`

Cerrar exige `Resolution` y notas. Asignaciones, estados, prioridad y resolución se guardan en `ReviewQueueHistory` y `SystemAuditLog`.

## Dashboard y API

El Centro de Riesgos muestra distribución, operaciones críticas/altas, revisión pendiente, factores principales y concentración de incidencias por fracción y aduana. Son indicadores internos basados exclusivamente en operaciones cargadas en SACE; no describen el universo externo ni atribuyen riesgo jurídico a una aduana o fracción.

Endpoints autenticados:

- `/api/batch-audits` y `/{id}`; acciones `start`, `cancel`, `retry-failed`; detalle `/operations`.
- `/api/operations/{id}/risk`, `/history` y `/calculate`.
- `/api/risk/dashboard`.
- `/api/review-queue` y `/{id}`; acciones `assign`, `status` y `priority`.

Las tablas usan `page`/`pageSize`, límite máximo configurable, filtros server-side y una lista cerrada de campos de ordenamiento. El dashboard obtiene la versión más reciente por operación, usa proyecciones sin tracking y evita consultas N+1.

## Seguridad y trazabilidad

Todos los endpoints requieren JWT. Los logs no contienen tokens, contraseñas ni contenido documental. Se registran las acciones `BATCH_AUDIT_*`, `BATCH_OPERATION_FAILED`, `RISK_SCORE_CALCULATED` y `REVIEW_*`. La migración `BatchAuditAndRiskScoringV1` agrega únicamente tablas e índices del sprint y su `Down` no toca operaciones, documentos, auditorías ni resultados LEGAL previos.

Configuración:

```json
{
  "BatchAudit": { "MaxOperationsPerBatch": 500 },
  "Pagination": { "MaxPageSize": 100 }
}
```

## Limitaciones

- El score no es conclusión jurídica, financiera ni predictiva.
- Los pesos V1 son operativos y sintéticos.
- No existe machine learning, IA generativa ni predicción.
- No se automatizan multas, sanciones ni acusaciones de fraude.
- No hay Redis, Kafka, RabbitMQ, WebSocket ni procesamiento distribuido.
- No existe un worker background dedicado; el piloto procesa dentro del backend y limita el tamaño del lote.
- El polling del frontend ocurre cada cuatro segundos sólo mientras hay un lote `RUNNING`.
- No se extrapolan resultados de proveedor, aduana o fracción fuera de los datos cargados en SACE.
- Proveedor no se muestra en el dashboard porque el modelo no tiene una dimensión estructurada confiable a nivel operación.
