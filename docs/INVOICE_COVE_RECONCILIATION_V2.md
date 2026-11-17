# Reconciliación Factura comercial ↔ COVE V2

## Alcance

`RECON-INV-COVE-V2` reconcilia una factura comercial con un COVE individual usando exclusivamente evidencia estructurada producida por Document Intelligence (`DocumentField`, `DocumentLineItem`, `DocumentParticipant` y `DocumentReference`). El resultado es técnico y no crea conclusiones LEGAL, Findings ni cambios de Risk.

La fuente funcional de esta versión es `FACT-COVE.docx`. El perfil V1 y sus resultados históricos permanecen disponibles sin modificar su algoritmo.

## Cardinalidad y emparejamiento

- La cardinalidad es una factura por COVE.
- Una sola factura y un solo COVE sólo constituyen `SINGLE_CANDIDATE`; sin evidencia explícita quedan `MANUAL_REVIEW` con `INSUFFICIENT_EVIDENCE`.
- `PAIR_CONFIRMED` requiere número de factura normalizado, referencia documental, `DocumentReference` o relación manual confirmada y auditada.
- Nunca se usa orden de carga, carpeta, nombre de archivo ni `First()` para decidir una pareja.
- Más de un candidato con evidencia produce `NO_VERIFICABLE` mediante `ReasonCode=AMBIGUOUS_PAIRING` y revisión humana.

## Emparejamiento de mercancías

`IInvoiceCoveLineMatcher` sólo acepta identificadores explícitos: número de parte, SKU/código de artículo, número de renglón explícito u otra combinación determinística de identificadores. Las descripciones, el orden de filas y la combinación cantidad/unidad nunca deciden por sí solos una pareja.

## Resultados funcionales

- `COINCIDE`
- `NO_COINCIDE`
- `FALTA_EN_FACTURA`
- `FALTA_EN_COVE`
- `NO_APLICA`
- `NO_VERIFICABLE`
- `ERROR_DE_LECTURA`

`NO_VERIFICABLE` es el único resultado visible para evidencia insuficiente. El detalle se conserva en `ReasonCode`, por ejemplo `INSUFFICIENT_EVIDENCE`, `AMBIGUOUS_PAIRING`, `CONVERSION_FACTOR_MISSING`, `ROLE_NOT_CONFIRMED` o `SEMANTIC_EQUIVALENCE_UNPROVEN`.

`FALTA_EN_FACTURA` y `FALTA_EN_COVE` sólo se producen cuando `ExpectedInLeft` o `ExpectedInRight` indican que el perfil exige el dato en ese documento. Los campos exclusivos y los destinados a otro documento se registran como `NO_APLICA` con tratamiento `DEFER_TO_OTHER_DOCUMENT`.

## Matriz y cálculos

El perfil cubre identificación y fecha, proveedor y domicilio desglosado, comprador/destino según rol, mercancías, cantidad/unidad, precios, importes, moneda y equivalentes USD. Los campos exclusivos de factura y COVE se conservan como evidencia informativa/diferida.

Para cada documento se persisten cálculos reproducibles en el snapshot:

- `QUANTITY * UNIT_PRICE = LINE_TOTAL` por renglón.
- `SUM(LINE_TOTAL) = COMMERCIAL_TOTAL` por documento.
- Fórmula, entradas, resultado calculado, valor declarado, diferencia, regla de redondeo, ubicación y resultado funcional.

## Cobertura e historial

La cobertura es el porcentaje de datos aplicables y esperados que pudieron evaluarse. Excluye `NO_APLICA` y no representa cumplimiento. Cada ejecución crea un resultado y snapshot nuevos; modificar posteriormente campos fuente no altera resultados históricos.

## Interfaz y API

Se reutilizan los endpoints de reconciliación existentes. La respuesta V2 agrega estado, método y motivo de emparejamiento; tratamiento y resultado funcional por dato; ubicaciones; `ReasonCode`; destino diferido; y cálculos reproducibles. La interfaz presenta un resumen integrado y un detalle consolidado en doce secciones.
