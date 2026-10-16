# Cross Validation Engine

## Objetivo

El motor de cruces de SACE compara datos técnicos extraídos de los documentos de una operación. Su propósito es señalar coincidencias, diferencias o falta de evidencia con trazabilidad reproducible. No interpreta obligaciones jurídicas ni emite conclusiones de cumplimiento legal.

## Arquitectura

El flujo está desacoplado mediante `IDocumentClassifier`, `IPdfTextExtractor`, `IDocumentFieldExtractor`, `IFieldNormalizationService` e `ICrossValidationEngine`:

```text
Documento
   ↓
Clasificación transparente
   ↓
Extracción de campos
   ↓
Normalización reproducible
   ↓
Regla CrossValidation versionada
   ↓
CrossValidationResult
```

Los documentos, campos, reglas y resultados se persisten en SQLite mediante EF Core. Las reglas se seleccionan por código, versión, vigencia y estado activo.

## Diferencia entre validación documental y LEGAL

`CrossValidationResult` representa una comparación técnica entre evidencias. `AuditResult` con reglas `LEGAL-*` pertenece al motor normativo y conserva su semántica independiente.

- Un `MISMATCH` no genera `NON_COMPLIANT`.
- El motor de cruces no crea `Finding` legal.
- La ausencia de evidencia no se convierte en diferencia: produce el outcome configurado.
- Las reglas `CrossValidationRule` no se almacenan en `NormativeRule`.

En forma explícita: `CrossValidationResult != AuditResult LEGAL`.

## DocumentField

`DocumentField` conserva el documento propietario, código del campo, valor original, valor normalizado, tipo, unidad, moneda, confianza, método, localizador, grupo, secuencia y confirmación manual. Una reextracción reemplaza únicamente campos automáticos; los campos confirmados manualmente se preservan.

## FieldCode

`DocumentFieldCodes` centraliza los identificadores compartidos. Sprint 1 utiliza, entre otros: `INVOICE_NUMBER`, `DATE`, `DESCRIPTION`, `QUANTITY`, `UNIT`, `TOTAL_AMOUNT`, `COMMERCIAL_VALUE`, `CURRENCY`, `INCOTERM`, `GROSS_WEIGHT`, `PACKAGE_COUNT`, `COVE_NUMBER` y `PEDIMENTO_NUMBER`.

## ExtractionMethod

- `XML`: metadatos o referencias obtenidos de XML reconocido.
- `PDF_TEXT`: texto embebido leído del PDF.
- `FILENAME`: reservado para evidencia derivada del nombre del archivo.
- `EXISTING_METADATA`: información estructurada ya capturada en `TradeOperation`.
- `MANUAL`: valor confirmado o capturado por una persona.

El fallback de pedimento basado en `TradeOperation` se identifica como `EXISTING_METADATA`, deja los identificadores de campo/documento correspondientes en `null` cuando no existen y agrega una explicación que aclara que el valor no fue extraído del archivo.

## Normalización

La normalización conserva siempre `RawValue` y produce `NormalizedValue`:

- Texto: `Trim`, espacios consecutivos reducidos y mayúsculas invariantes. No elimina acentos.
- Identificadores: mayúsculas; pedimento, fracción y NICO también eliminan separadores visuales.
- Decimales/cantidades: separadores de miles y decimal se interpretan de forma determinista y se guardan con cultura invariante.
- Moneda: equivalencias explícitas como `US$ → USD` y `MX$ → MXN`.
- Unidad: catálogo pequeño y explícito, por ejemplo `PIEZAS → PZA` y `KGS → KG`.
- Peso: gramos se convierten a kilogramos; otras unidades se conservan si no existe conversión inequívoca.
- Fecha: formato ISO `yyyy-MM-dd` cuando puede interpretarse sin ambigüedad.

`NORMALIZED_TEXT` exige igualdad del texto normalizado, ignorando únicamente diferencias de mayúsculas. No usa `contains`, fuzzy matching, IA, embeddings ni un umbral oculto.

## Clasificación documental

La clasificación combina señales explicables del nombre, texto PDF y estructura XML. Los pesos y umbrales están en configuración. La respuesta incluye tipo propuesto, confianza, banda y evidencias. Una propuesta ambigua o débil requiere selección manual y no inventa un tipo.

Sprint 1 reconoce además `CARTA_318`, `DECLARACION_INCREMENTABLES`, `CARTA_ENCOMIENDA` y `MANIFESTACION_VALOR_MV_MVE`. Los códigos históricos permanecen disponibles para compatibilidad.

## PdfPig / limitaciones PDF

SACE usa `PdfPig` 0.1.16, licencia Apache-2.0, detrás de `IPdfTextExtractor`. La restauración selecciona un ensamblado compatible y la solución compila sobre .NET 10. Las excepciones de lectura se convierten en texto vacío para no cancelar la carga del archivo; posteriormente la clasificación o el cruce reporta evidencia insuficiente.

PdfPig solo lee la capa textual existente. Sprint 1 no incluye OCR, separación multipágina, interpretación visual ni validación de autenticidad del PDF.

## CrossValidationRule

Cada regla especifica código, versión, nombre, categoría, documento/campo origen, documento/campo destino, tipo de comparación, tolerancias, severidad, comportamiento ante evidencia faltante, estado y vigencia.

## Versionado de reglas

El índice único `Code + Version` impide duplicados. El seed inserta solo combinaciones inexistentes. Para una operación se filtran reglas activas y vigentes y se ejecuta únicamente la versión más alta de cada código.

## ComparisonType

- `EXACT_TEXT`: igualdad exacta del valor original.
- `NORMALIZED_TEXT`: igualdad del valor normalizado.
- `NORMALIZED_IDENTIFIER`: igualdad del identificador normalizado.
- `EXACT_NUMERIC`: igualdad decimal exacta.
- `NUMERIC_TOLERANCE`: diferencia dentro de una tolerancia absoluta.
- `PERCENT_TOLERANCE`: diferencia porcentual dentro del porcentaje configurado.
- `CURRENCY_AMOUNT`: igualdad decimal de importes; la moneda se valida mediante una regla separada.
- `DATE` y `PRESENCE`: disponibles en el modelo para reglas versionadas posteriores; no forman parte de las 18 reglas de Sprint 1.

## Tolerancias

Las tolerancias usan `decimal` y se almacenan en la regla. `CROSS-PL-PED-WEIGHT-001` y `CROSS-PL-TR-WEIGHT-001` declaran explícitamente `PercentTolerance = 2`; no existe una tolerancia escondida en el evaluador.

## CrossValidationResult

El resultado conserva operación, auditoría opcional, regla y versión, documentos y campos participantes, valores raw/normalizados, outcome, diferencias absoluta/porcentual, confianza, explicación y fecha. La API también expone el método de extracción de cada lado; el fallback estructurado se muestra como `EXISTING_METADATA`.

## Outcomes

- `MATCH`: existen ambas evidencias y cumplen la comparación y tolerancia.
- `MISMATCH`: existen evidencias comparables y no cumplen la regla.
- `INSUFFICIENT_EVIDENCE`: falta una o ambas evidencias necesarias.
- `MANUAL_REVIEW`: los valores existen pero no son interpretables de forma inequívoca, o así lo exige la regla.
- `NOT_APPLICABLE`: la política de evidencia faltante indica que la regla no aplica.

## Reglas Sprint 1

Todas usan `MissingEvidenceBehavior = INSUFFICIENT_EVIDENCE`.

| Código | Origen | Destino | ComparisonType | Tolerancia |
|---|---|---|---|---|
| `CROSS-INV-PED-INVOICE-001` | Factura / `INVOICE_NUMBER` | Pedimento / `INVOICE_NUMBER` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-PED-QTY-001` | Factura / `QUANTITY` | Pedimento / `QUANTITY` | `EXACT_NUMERIC` | 0 |
| `CROSS-INV-PED-UNIT-001` | Factura / `UNIT` | Pedimento / `UNIT` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-PED-VALUE-001` | Factura / `TOTAL_AMOUNT` | Pedimento / `COMMERCIAL_VALUE` | `CURRENCY_AMOUNT` | 0 |
| `CROSS-INV-PED-CURRENCY-001` | Factura / `CURRENCY` | Pedimento / `CURRENCY` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-PED-INCOTERM-001` | Factura / `INCOTERM` | Pedimento / `INCOTERM` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-COVE-INVOICE-001` | Factura / `INVOICE_NUMBER` | COVE / `INVOICE_NUMBER` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-COVE-DESC-001` | Factura / `DESCRIPTION` | COVE / `DESCRIPTION` | `NORMALIZED_TEXT` | igualdad normalizada |
| `CROSS-INV-COVE-QTY-001` | Factura / `QUANTITY` | COVE / `QUANTITY` | `EXACT_NUMERIC` | 0 |
| `CROSS-INV-COVE-UNIT-001` | Factura / `UNIT` | COVE / `UNIT` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-INV-COVE-VALUE-001` | Factura / `TOTAL_AMOUNT` | COVE / `TOTAL_AMOUNT` | `CURRENCY_AMOUNT` | 0 |
| `CROSS-INV-COVE-CURRENCY-001` | Factura / `CURRENCY` | COVE / `CURRENCY` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-PL-PED-DESC-001` | Packing / `DESCRIPTION` | Pedimento / `DESCRIPTION` | `NORMALIZED_TEXT` | igualdad normalizada |
| `CROSS-PL-PED-QTY-001` | Packing / `QUANTITY` | Pedimento / `QUANTITY` | `EXACT_NUMERIC` | 0 |
| `CROSS-PL-PED-UNIT-001` | Packing / `UNIT` | Pedimento / `UNIT` | `NORMALIZED_IDENTIFIER` | — |
| `CROSS-PL-PED-WEIGHT-001` | Packing / `GROSS_WEIGHT` | Pedimento / `GROSS_WEIGHT` | `PERCENT_TOLERANCE` | 2% explícito |
| `CROSS-PL-TR-PACKAGES-001` | Packing / `PACKAGE_COUNT` | Transporte / `PACKAGE_COUNT` | `EXACT_NUMERIC` | 0 |
| `CROSS-PL-TR-WEIGHT-001` | Packing / `GROSS_WEIGHT` | Transporte / `GROSS_WEIGHT` | `PERCENT_TOLERANCE` | 2% explícito |

## Flujo de ejecución

1. La carga valida y almacena el documento.
2. El clasificador propone un tipo con evidencia y confianza.
3. El extractor conserva campos manuales y renueva campos automáticos.
4. El normalizador genera valores comparables sin perder los originales.
5. El motor selecciona una versión activa por código.
6. Evalúa evidencia, tipo de comparación y tolerancia.
7. Persiste snapshots y registra inicio/finalización en la bitácora.
8. La UI consulta el último resultado de cada regla.

## API

Todas estas rutas requieren JWT porque pertenecen al grupo `/api` autenticado:

- `GET /api/operations/{operationId}/document-fields`
- `POST /api/operations/{operationId}/document-fields/reextract`
- `GET /api/operations/{operationId}/cross-validations`
- `POST /api/operations/{operationId}/cross-validations/run`

La extracción también se ejecuta al cargar un documento. La auditoría diaria existente ejecuta los cruces y devuelve sus conteos. `/health` y `/api/health` permanecen públicos.

## Frontend

El detalle de la operación incluye la pestaña **Cruces documentales**, agrupada por categoría. Muestra outcome mediante texto y color, valores original/normalizado, método de extracción, documento, diferencia, confianza y explicación. Soporta estados vacíos y resultados con evidencia insuficiente, revisión manual o no aplicable.

El detalle documental muestra los `DocumentField`. La selección manual de clasificación continúa disponible.

## Auditoría mensual

La regla mensual DEMO cuenta una operación del periodo una sola vez cuando posee al menos una auditoría no mensual completada. Expone importaciones, exportaciones, fracciones distintas, desglose por fracción y filas de detalle. Operaciones no auditadas o fuera del periodo quedan excluidas.

## Trazabilidad

La bitácora registra `DOCUMENT_FIELDS_EXTRACTED`, `CROSS_VALIDATION_STARTED`, `CROSS_VALIDATION_COMPLETED` y `CROSS_VALIDATION_REEXTRACTED`. Antes de asociar el log se comprueba que el usuario siga existiendo; una sesión antigua no provoca una violación de llave foránea y el evento puede quedar atribuido al sistema.

Los resultados guardan valores y versión de regla usados en el momento de evaluación, aunque después cambien los documentos o reglas.

## Seguridad

- JWT obligatorio para leer campos/resultados o ejecutar extracción/cruces.
- Validación existente de extensión, MIME, tamaño, firma y estructura.
- Archivos fuera de la raíz pública y descarga autenticada.
- Nombres físicos aleatorios y protección de rutas.
- Logs sin tokens, contraseñas ni contenido documental completo.
- Las bases y archivos temporales de prueba se crean fuera de la base real y se eliminan al finalizar.

## Limitaciones

- Extracción PDF basada en patrones y capa textual; no OCR.
- Cobertura de campos deliberadamente acotada.
- Igualdad textual estricta después de normalización; no equivalencia semántica.
- Importes y monedas se comparan mediante reglas separadas.
- SQLite y almacenamiento local continúan orientados al piloto.
- No existen consultas a SAT, ANAM o VUCEM.

## Roadmap

El siguiente paso recomendado es validar patrones, campos y tolerancias con expedientes sintéticos representativos y usuarios especialistas antes de proponer un Sprint 2. OCR, integraciones externas, IA y nuevas reglas quedan fuera de Sprint 1.
