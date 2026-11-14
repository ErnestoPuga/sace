# Document Intelligence Layer V1

## Alcance

Esta capa transversal convierte documentos heterogéneos en evidencia técnica canónica y trazable antes de cualquier reconciliación. No sustituye ni adelanta las reglas finales Factura ↔ COVE o Factura ↔ Packing List, no modifica resultados `LEGAL`, no crea `Finding` y no alimenta una conclusión de incumplimiento por sí sola.

Las matrices funcionales de referencia son los documentos **FACT-COVE** y **PACKING LIST - FACT**. El diseño no contiene coordenadas fijas, regex específicas de un proveedor ni una suposición universal de relación 1:1.

## Flujo

```text
OperationDocument
       ↓
lectura nativa por página y región
       ↓ texto insuficiente
OCR local opcional y feature-flagged
       ↓
clasificación semántica + idioma + layout fingerprint
       ↓
alias/contexto/forma del valor + mapping aprobado exacto
       ↓
DocumentField + DocumentLineItem + DocumentParticipant
       ↓
campos calculados reproducibles
       ↓
snapshot versionado + candidatos de relación + corrección humana
```

## Modelo canónico

- `CanonicalFieldDefinition` define código, tipo, categoría, repetibilidad y unidad canónica.
- `FieldAlias` conserva alias multilingües y opcionalmente acotados por tipo/proveedor.
- `UnitAlias` distingue unidades comerciales y conversiones controladas. Por ejemplo, `PCS`/`PZA` se homologan a `PZA`, mientras `PAIRS` se conserva como `PAR`; igualdad numérica no implica igualdad de unidad.
- `DocumentField` conserva etiqueta y texto original, valor normalizado, página, región, tabla/fila/columna, fuente (`EXTRACTED`, `CALCULATED`, `INFERRED`, `MANUAL`), confianza, método, fórmula y motivo.
- `DocumentLineItem` conserva partidas, página/región, datos comerciales y logísticos sin asumir el mismo orden entre documentos.
- `DocumentParticipant` mantiene roles separados (`SELLER`, `MANUFACTURER`, `SHIPPER`, `BUYER`, `SHIP_TO`, `CONSIGNEE`, etc.).
- `DocumentIntelligenceAnalysis` es el snapshot inmutable de cada ejecución.
- `DocumentFieldCorrection` conserva antes/después, usuario, fecha, motivo y evidencia.
- `ProviderDocumentProfile` y `ApprovedFieldMapping` son pistas gobernadas; sólo se aplican con tipo y layout fingerprint exactos.
- `DocumentRelationshipCandidate` sugiere relaciones mediante referencias tipadas compartidas, sin confirmar por sí solo cardinalidad o alcance.

## Lectura, OCR y degradación segura

`IPdfLayoutExtractor` usa PdfPig y conserva página y región. Si el texto nativo no alcanza el umbral, `IOcrTextExtractor` puede invocar un ejecutable o wrapper local/on-prem configurado para aceptar el archivo y devolver texto por salida estándar.

El OCR está deshabilitado por omisión:

```json
{
  "DocumentIntelligence": {
    "Ocr": {
      "Enabled": false,
      "ExecutablePath": "",
      "Arguments": "\"{input}\" stdout -l {languages}",
      "Languages": ["spa", "eng"]
    }
  }
}
```

No existe integración cloud. Si el adaptador no está instalado o falla, el análisis termina con estado controlado (`OCR_UNAVAILABLE`, `DOCUMENT_UNREADABLE` o `ERROR_DE_LECTURA`), sin inventar campos ni transformar la ausencia en `NON_COMPLIANT`.

## Resolución semántica y cálculos

El resolver combina alias, contexto, forma del valor, unidad, tipo documental y mappings humanos aprobados. Etiquetas ambiguas como `Amount`, `Units` o `To` no se asignan sin contexto suficiente. `Line Amount` y `Invoice Total` conservan alcances distintos.

Los cálculos se marcan como `CALCULATED`, guardan fórmula y nunca reemplazan un valor extraído:

- `UNIT_PRICE = LINE_TOTAL / QUANTITY`, si el precio no existe; si existe, sólo se valida consistencia.
- `VOLUME_CBM = LENGTH × WIDTH × HEIGHT`, únicamente cuando las tres dimensiones comparten una unidad verificable (`M`, `CM` o `MM`).

No se infiere automáticamente piezas mediante cartones × piezas por cartón sin una composición homogénea acreditada.

## API y UI

Endpoints autenticados:

- `GET /api/operations/{operationId}/document-intelligence`
- `GET /api/documents/{documentId}/intelligence`
- `POST /api/documents/{documentId}/reanalyze`
- `POST /api/document-fields/{fieldId}/correct`
- `GET /api/document-intelligence/canonical-fields`
- `GET /api/document-intelligence/provider-profiles`

La pantalla **Inteligencia documental** permite seleccionar operación/documento, revisar método, OCR, estado, campos, ubicación, roles e historial, reanalizar y registrar correcciones humanas.

## Límites V1

- No hay entrenamiento de modelos ni servicios cloud.
- No se afirma correspondencia definitiva entre facturas, COVE o packing lists.
- No se calcula equivalencia de unidades sin factor verificado.
- La evidencia técnica permanece separada de LEGAL y Risk; los motores consumidores conservan sus propias reglas y gobernanza.

