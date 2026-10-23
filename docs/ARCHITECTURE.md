# Arquitectura SACE MVP

## Gobernanza normativa oficial

La capa RRNA incorpora `OfficialLegalSource` → `OfficialLegalSourceVersion` → `OfficialSourceCitation`, vinculada a requisitos mediante una tabla puente. `RegulatoryTariffEntry` conserva fracción cruda/normalizada y NICO por separado. `IRegulatoryCatalogImportService` valida e importa paquetes atómicos; `IRegulatoryCatalogDiffService` compara versiones y estima impacto sin mutar auditorías. Esta capa no depende de `ILegalAuditEngine` ni genera hallazgos.

SACE es un monolito modular: React consume una API ASP.NET Core; EF Core persiste en SQLite y `IFileStorageService` guarda archivos fuera de la raíz pública. Las reglas DEMO se resuelven en `IAuditEngine` y la consolidación mensual en `IMonthlyAuditRule`, de modo que ambas implementaciones puedan sustituirse sin acoplarlas a endpoints o UI.

El módulo documental conserva esa separación:

- `IDocumentProcessingService` orquesta análisis y persistencia por archivo y mantiene delgados los endpoints individual y masivo.
- `IDocumentValidationService` aplica límites, MIME, firma/estructura y códigos técnicos de rechazo.
- `IDocumentClassifier` propone tipos con reglas transparentes de nombre y estructura XML, siempre con confianza y motivo editables.
- `IXmlDocumentProcessor` identifica únicamente formatos conocidos y extrae metadata/referencias confiables.
- `DocumentReference` asocia técnicamente documentos mediante COVE, e-Document, UUID CFDI, DODA u otros identificadores.
- SHA-256 identifica contenido repetido dentro de la misma operación; no participa en autenticación.

## Motor de cruces documentales

Sprint 1 incorpora un flujo técnico separado del motor normativo:

```text
Document processing
        ↓
DocumentField extraction
        ↓
Normalization
        ↓
Cross Validation Engine
        ↓
CrossValidationResult

Normative Engine
        ↓
LEGAL AuditResult
```

`CrossValidationResult != AuditResult LEGAL`. Una diferencia documental no cambia el resultado jurídico, no genera `NON_COMPLIANT` y no crea hallazgos LEGAL. Las reglas de cruce se almacenan en `CrossValidationRule`, con código/versión, vigencia y tolerancias independientes de `NormativeRule`.

`IPdfTextExtractor` aísla PdfPig; `IDocumentFieldExtractor` conserva el valor original y su procedencia; `IFieldNormalizationService` produce valores comparables; `ICrossValidationEngine` guarda resultados trazables. Los detalles, reglas y limitaciones están en [CROSS_VALIDATION_ENGINE.md](CROSS_VALIDATION_ENGINE.md).

> Las reglas DEMO no constituyen una interpretación jurídica ni validación oficial de cumplimiento aduanero.

## Valoración aduanera y contribuciones

Sprint 2 extiende el flujo técnico después de los cruces documentales:

```text
Document processing → DocumentField → Cross Validation
                                           ↓
                                  Customs Valuation
                                           ↓
                              Contribution Reconciliation

LEGAL AuditResult (independiente)
```

`CustomsValuationCalculation` conserva versiones, fórmula, tolerancias y snapshot. Sus `CustomsValuationComponent` representan conceptos económicos únicos y `ValuationComponentEvidence` agrupa evidencia primaria, soporte, pago y declaración sin multiplicar importes. `ContributionCalculation` y `ContributionLine` concilian importes técnicos conservando códigos desconocidos. Ninguno de estos resultados crea hallazgos ni modifica el motor LEGAL. Véase [CUSTOMS_VALUATION_ENGINE.md](CUSTOMS_VALUATION_ENGINE.md).

## RRNA y cumplimiento regulatorio documental

Sprint 3 añade una etapa posterior a contribuciones y paralela a LEGAL:

```text
Operación + DocumentField + documentos clasificados
                       ↓
        Regulatory Requirement Engine
                       ↓
 RegulatoryRequirementEvaluation + Evidence

LEGAL AuditResult (independiente y sin mutaciones)
```

`RegulatoryRequirement` conserva código, versión, estado y vigencia; sus condiciones son tipadas y sus grupos permiten AND/OR controlado. `RegulatoryRequirementEvaluation` es histórica, usa la fecha de operación y conserva el resultado automático aunque exista revisión humana. Esta capa no crea `Finding` ni transforma ausencia documental en `NON_COMPLIANT`. Véase [REGULATORY_REQUIREMENTS_ENGINE.md](REGULATORY_REQUIREMENTS_ENGINE.md).
