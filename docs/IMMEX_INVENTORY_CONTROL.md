# Control IMMEX / Anexo 24 V1

## Objetivo y alcance

El módulo ofrece trazabilidad operativa de importaciones temporales vinculadas explícitamente a un programa IMMEX. Registra entradas, consumos, salidas, descargos PEPS, saldos, temporalidades técnicas, conciliaciones e incidencias. Es una primera versión funcional: no certifica el Anexo 24, no sustituye un ERP/MRP ni automatiza conclusiones jurídicas.

La base normativa registrada y versionada comprende el Anexo 24 de las RGCE 2026, la regla 4.3.1, el artículo 108 de la Ley Aduanera vigente y una referencia oficial al Decreto IMMEX. Cada regla `VERIFIED` conserva una cita oficial. La presencia de una cita permite explicar el cálculo; no amplía por inferencia el universo de mercancías o excepciones modeladas.

## Matriz de cobertura

| Área | Estado V1 | Implementación |
| --- | --- | --- |
| Entradas temporales | `SUPPORTED` | `ImmexInventoryEntry`, importación idempotente y reporte de entradas |
| Salidas y descargos | `SUPPORTED` | `ImmexInventoryExit`, preview/confirm PEPS y reporte de salidas |
| Saldos | `SUPPORTED` | Saldo persistido, conciliación y reporte agregado sin duplicación por líneas |
| Consumos, merma y desperdicio | `SUPPORTED` | Captura/consulta, conceptos separados y reporte |
| Activo fijo | `PARTIAL` | Registro y consulta en flujo separado; catálogo de movimientos no exhaustivo |
| Mercancías sensibles y plazos especiales | `MANUAL` | Overrides versionados posibles; catálogo especial no exhaustivo |
| SECIIT completo | `NOT_IMPLEMENTED` | Fuera de alcance |
| Cobertura específica de empresas certificadas | `NOT_IMPLEMENTED` | Fuera de alcance |

## Programa, materiales y productos

`ImmexProgram` almacena número, RFC, empresa, estado declarado, vigencia, modalidad, domicilios y procedencia. El número no implica estado `ACTIVE`: ese estado debe capturarse expresamente. Los materiales y productos pertenecen a un programa, usan un código estable, fracción, NICO y unidades TIGIE/comercial. Las operaciones de alta y actualización generan bitácora.

Una `TradeOperation` sólo se importa cuando tiene `ImmexProgramId` e `IsTemporaryImport == true`. El sistema no infiere temporalidad por la clave de pedimento ni por la existencia de un documento IMMEX. La importación reutiliza campos documentales disponibles, crea una entrada por secuencia y es idempotente por operación/secuencia.

Si la evidencia importada no identifica material o clasificación, la entrada queda en revisión. La confirmación explícita vincula un material activo del mismo programa y selecciona la clasificación; sólo entonces se recalcula la temporalidad. El sistema no asigna por omisión la clasificación `COMPONENT` ni un plazo de 18 meses.

## Entradas y consumos

Cada entrada conserva pedimento, partida, fecha, material, fracción/NICO, clasificación, cantidad original, saldo disponible, unidad, regla temporal aplicada y estado. El detalle muestra una línea de tiempo con entrada, consumos, descargos y saldo actual.

`ImmexMaterialUsage` registra periodo, material, producto opcional, cantidad utilizada, merma (`LossQuantity`), desperdicio (`WasteQuantity`), unidad y fuente. Merma y desperdicio son conceptos independientes: no se suman ni se sustituyen entre sí. V1 admite alta y consulta; no infiere consumos desde producción.

## Salidas, PEPS y atomicidad

La salida identifica programa, tipo, pedimento, fecha, material o producto, cantidad y unidad. El motor usa PEPS/FIFO por `EntryDate`, `Sequence` e `Id`, únicamente sobre entradas elegibles del mismo programa y material. Los activos fijos se excluyen.

`Preview` calcula asignaciones sin modificar saldos y deja un evento de auditoría. `Confirm` repite la validación dentro de una transacción serializable, crea la salida y sus líneas, y actualiza saldos de forma atómica. Si otra operación cambió el lote, se cancela la transacción. Un descargo parcial nunca genera saldos negativos: la cantidad no asignada queda registrada y produce un issue técnico.

Las reversiones no eliminan líneas históricas. `ImmexDischargeReversal` conserva la razón, el usuario y la fecha, restaura el saldo permitido y marca la línea como revertida.

## Unidades y conversiones

El descargo directo exige unidades iguales. Una unidad distinta requiere una conversión vigente y `IsVerified=true`, específica del material. La conversión conserva unidad origen/destino, factor, vigencia y fuente. La UI permite listar y crear conversiones porque ésas son las únicas operaciones soportadas por la API; no ofrece edición ni eliminación. SACE nunca propone factores.

## Activo fijo

`ImmexFixedAsset` registra descripción, marca, modelo, serie, fracción/NICO, pedimento, fecha, estado y datos de salida. Se administra separado de materiales consumibles y no participa en PEPS. La cobertura permanece `PARTIAL` porque V1 no implementa un catálogo exhaustivo de movimientos y excepciones de activo fijo.

## Temporalidades

Las reglas son versionadas, tienen vigencia, clasificación, prioridad, estado de verificación y cita. Las reglas generales modeladas son 18 meses para componentes, 2 años para contenedores y vigencia del programa para activo fijo/maquinaria/equipo. Un override `VERIFIED` de mayor prioridad prevalece. Las reglas verificadas se muestran como solo lectura.

El cálculo usa aritmética de calendario, conserva `RuleCode`, versión, `DueDate`, outcome, explicación y cita. Los umbrales `DueSoonDays` y `CriticalDays` son configurables. Los resultados posibles incluyen `ACTIVE`, `DUE_SOON`, `DUE_CRITICAL`, `PAST_DUE`, `RESOLVED`, `INSUFFICIENT_DATA` y `MANUAL_REVIEW`. Una clasificación ambigua no recibe automáticamente 18 meses.

`PAST_DUE` es una señal técnica: no equivale a `NON_COMPLIANT`. Al quedar el saldo en cero, la temporalidad se resuelve operativamente.

## Saldos, conciliación e issues

El saldo se persiste por entrada. La conciliación recalcula `OriginalQuantity - descargos no revertidos`, detecta saldo negativo, diferencias y salidas huérfanas, pero nunca autocorrige cantidades. Los issues conservan código, severidad, estado, entrada/salida, descripción, fecha y resolución.

La UI permite `OPEN → IN_REVIEW → RESOLVED` y cierre `DISMISSED`. `RESOLVED` y `DISMISSED` requieren texto de resolución. Esta resolución es operativa, no crea `Finding` y no cambia resultados LEGAL.

## Reportes y CSV

La API expone reportes de entradas, salidas, saldos y consumos. Todos aceptan filtros de programa y periodo; consumos añade material/producto. JSON incluye un aviso de alcance. CSV usa UTF-8 con BOM, encabezados explícitos, escape por comillas y dataset vacío válido. El reporte de saldos agrupa desde entradas y no multiplica cantidades por líneas de descargo.

## Risk V2 y cola de revisión

`RISK-OPS-V2` copia los factores históricos de V1 y añade señales IMMEX: temporalidad próxima/crítica/vencida, intento de saldo negativo, cantidad no asignada, salida huérfana, unidades incompatibles e inconsistencias de conciliación. Las señales de temporalidad del mismo lote se correlacionan por máximo. V1 permanece intacto.

Un score combinado `HIGH` o `CRITICAL` reutiliza `ReviewQueueItem`; no existe una cola IMMEX paralela. El score sirve para priorización operativa y no altera `AuditResult`, `Finding` ni `ILegalAuditEngine`.

## API y frontend

Las rutas autenticadas bajo `/api/immex` cubren programas, catálogos, inventario, importación, consumos, descargos, temporalidades, activo fijo, conversiones, issues, dashboard, conciliación y reportes. Los errores conocidos se devuelven como `ApiError` sin stack trace.

El frontend incluye dashboard, inventario paginado, detalle/timeline, preview y confirmación de descargos, activo fijo, programas/materiales/productos, conversiones, reglas, consumos, issues y reportes. Cada lista presenta un estado vacío comprensible. Los filtros y la paginación de inventario, consumos e issues se ejecutan en servidor; los reportes usan proyecciones de EF Core y `AsNoTracking`.

## Seguridad y auditoría

Todas las rutas requieren JWT. Los eventos IMMEX conservan usuario cuando la acción proviene de una sesión y se almacenan en `SystemAuditLog`. Preview no cambia inventario, aunque registra la acción. Confirmaciones y reversiones son transaccionales. Los archivos y secretos siguen las exclusiones generales del repositorio.

## Limitaciones

- No es ERP/MRP ni SECIIT completo.
- No consulta VUCEM, SAT ni Secretaría de Economía en línea.
- No contiene un catálogo completo de temporalidades especiales ni de mercancías sensibles.
- No automatiza conclusiones LEGAL ni certifica cumplimiento del Anexo 24.
- No existe todavía integración productiva on-premise.
- El bundle frontend requiere optimización posterior mediante code splitting.
- La revisión de vulnerabilidades y upgrades de dependencias queda para la fase final indicada por el plan del proyecto.
