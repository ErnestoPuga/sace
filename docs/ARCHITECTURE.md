# Arquitectura SACE MVP

SACE es un monolito modular: React consume una API ASP.NET Core; EF Core persiste en SQLite y `IFileStorageService` guarda archivos fuera de la raíz pública. Las reglas DEMO se resuelven en `IAuditEngine` y la consolidación mensual en `IMonthlyAuditRule`, de modo que ambas implementaciones puedan sustituirse sin acoplarlas a endpoints o UI.

El módulo documental conserva esa separación:

- `IDocumentProcessingService` orquesta análisis y persistencia por archivo y mantiene delgados los endpoints individual y masivo.
- `IDocumentValidationService` aplica límites, MIME, firma/estructura y códigos técnicos de rechazo.
- `IDocumentClassifier` propone tipos con reglas transparentes de nombre y estructura XML, siempre con confianza y motivo editables.
- `IXmlDocumentProcessor` identifica únicamente formatos conocidos y extrae metadata/referencias confiables.
- `DocumentReference` asocia técnicamente documentos mediante COVE, e-Document, UUID CFDI, DODA u otros identificadores.
- SHA-256 identifica contenido repetido dentro de la misma operación; no participa en autenticación.

> Las reglas DEMO no constituyen una interpretación jurídica ni validación oficial de cumplimiento aduanero.
