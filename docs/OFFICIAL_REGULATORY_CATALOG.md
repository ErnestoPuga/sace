# Catálogo normativo oficial V1

Sprint 4 agrega gobierno de fuentes oficiales sin convertir a SACE en asesor jurídico. El catálogo registra fuente, versión, vigencia, cita exacta, regla, fracción arancelaria, NICO y acotación como piezas separadas y auditables.

## Estados y verificación

- Una fuente y cada versión recorren `STAGED`, `REVIEWED`, `VERIFIED`, `SUPERSEDED` o `REJECTED`.
- Una regla sólo puede pasar de `DRAFT` a `VERIFIED` si tiene al menos una cita perteneciente a una versión de fuente `VERIFIED` y su vigencia no se traslapa con otra versión verificada del mismo código.
- Una versión `VERIFIED` no se edita directamente. Las modificaciones crean versiones nuevas y preservan evaluaciones históricas.
- `OFFICIAL_SOURCE`, `OFFICIAL_COMPILATION` y `MANUAL_VERIFICATION` registran el nivel de verificación.

## Fuentes iniciales

La semilla registra metadatos versionables de RGCE 2026 y Anexo 22 desde el portal oficial del SAT. El catálogo operativo inicial contiene exactamente dos reglas reales verificadas derivadas del Acuerdo publicado en el DOF el 28 de septiembre de 2026:

- `RRNA-NOM-221-2-SCFI-2018`, fracción `8517.13.01`, vigente específicamente desde el 9 de noviembre de 2026.
- `RRNA-NOM-033-ENER-2019`, fracción `8501.10.10`, NICO `06`, `07` y `99`, vigente desde el 29 de septiembre de 2026.

Las acotaciones técnicas se preservan literalmente en `QualificationText`. Cuando el expediente no contiene atributos suficientes para resolverlas, una coincidencia arancelaria produce `MANUAL_REVIEW`; nunca se infiere aplicabilidad material.

## Paquete JSON formal

`schemaVersion` debe ser `1`. El paquete contiene una fuente, una versión, citas con claves locales y requisitos que referencian esas claves. Cada entrada arancelaria conserva fracción cruda, NICO, alcance, régimen, países, vigencia, tipo/texto de acotación y cita.

```json
{
  "schemaVersion": 1,
  "packageId": "SE-RRNA-2026-09-28-v1",
  "sourceCode": "DOF-SE-RRNA-2026-09-28",
  "sourceTitle": "Título oficial",
  "sourceType": "DOF_PUBLICATION",
  "authority": "Secretaría de Economía",
  "officialUrl": "https://dof.gob.mx/...",
  "versionLabel": "Publicación DOF 28/09/2026",
  "publicationDate": "2026-09-28T00:00:00Z",
  "effectiveFrom": "2026-09-29T00:00:00Z",
  "effectiveTo": null,
  "verificationStatus": "VERIFIED",
  "citations": [{ "key": "ANNEX-241-1", "citationType": "NUMERAL", "citationLabel": "Anexo 2.4.1, numeral 1" }],
  "requirements": []
}
```

El Dry Run valida esquema, URL oficial HTTPS `gob.mx`, estado verificado, citas, fracciones de ocho dígitos, NICO de dos dígitos, duplicados, vigencias y acotaciones. No escribe entidades de catálogo; únicamente registra el evento de auditoría exigido. La importación calcula SHA-256, rechaza paquetes repetidos y ejecuta todos los cambios en una transacción.

## Diff e impacto

`IRegulatoryCatalogDiffService` clasifica requisitos como `ADDED`, `REMOVED`, `MODIFIED` o `UNCHANGED`, considerando condiciones, evidencia, fracciones, NICO, acotaciones y vigencias. El impacto informa operaciones históricas y posteriores potencialmente relacionadas; no reaudita ni modifica resultados.

## API autenticada

- `GET/POST /api/legal-sources`
- `GET /api/legal-sources/{id}`
- `POST /api/legal-sources/{id}/versions`
- `POST /api/legal-sources/{id}/verify`
- `GET /api/legal-sources/{id}/diff`
- `POST /api/regulatory-catalog/import/dry-run`
- `POST /api/regulatory-catalog/import`
- `GET /api/regulatory-catalog/imports[/{id}]`
- `POST /api/regulatory-requirements/{id}/verify`

El motor RRNA continúa separado de `ILegalAuditEngine`: no genera hallazgos, no cambia el resultado LEGAL y toda indeterminación queda en revisión manual.
