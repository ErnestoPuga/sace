# SACE

La guía para publicar el piloto en Render y Vercel está en [docs/DEPLOYMENT_PILOT.md](docs/DEPLOYMENT_PILOT.md).

**Sistema Auditor de Comercio Exterior** es un MVP de extremo a extremo para registrar expedientes, administrar documentos, evaluar controles normativos acotados, generar hallazgos, corregir documentos y conservar trazabilidad.

## Importante

> El motor LEGAL v1 automatiza únicamente los criterios expresamente documentados. No constituye una revisión integral ni sustituye revisión jurídica profesional.

Las reglas `DEMO-*` y controles `TECH-*` se conservan por compatibilidad, pero están excluidos del cálculo legal. Las reglas `LEGAL-*` tienen fundamento, vigencia, versión, evidencia y resultado trazable. Véase [docs/NORMATIVE_ENGINE.md](docs/NORMATIVE_ENGINE.md).

## Arquitectura

- Monorepo con backend, frontend, pruebas, almacenamiento y documentación.
- Backend: ASP.NET Core 10 Minimal API, C#, EF Core 10, SQLite, JWT y Swagger/OpenAPI.
- Frontend: React, TypeScript, Vite, React Router, TailwindCSS, Axios, Recharts y Lucide.
- Reglas DEMO/técnicas encapsuladas en `IAuditEngine`, motor normativo en `ILegalAuditEngine` y consolidación mensual en `IMonthlyAuditRule`.
- Procesadores futuros desacoplados mediante `IPedimentoParser`, `IXmlDocumentProcessor`, `IPdfDocumentProcessor` e `IExcelDocumentProcessor`.
- Almacenamiento desacoplado mediante `IFileStorageService`; la implementación MVP es local y usa nombres físicos aleatorios.
- Auditoría diaria programada mediante `DailyAuditWorker`, además de ejecución manual e inmediata al crear una operación.

Más detalle en [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Requisitos

- .NET SDK 10.0.303 o compatible.
- Node.js 20 o superior (validado con Node 24).
- npm 10 o superior.

## Cómo ejecutar backend

```powershell
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

La API queda en `http://localhost:5092` y Swagger en `http://localhost:5092/swagger`. Al arrancar, la aplicación aplica migraciones pendientes y genera los datos demo si la base está vacía.

## Cómo ejecutar frontend

```powershell
cd frontend
Copy-Item .env.example .env
npm install
npm run dev
```

La aplicación queda en `http://localhost:5173`. `VITE_API_URL` permite cambiar la URL de la API.

## Base de datos

SQLite crea `backend/sace.db`. El modelo incluye usuarios, operaciones, tipos documentales, versiones de documentos, auditorías, resultados, hallazgos, historial de hallazgos, reglas versionadas y bitácora del sistema.

Los archivos se guardan fuera de la raíz pública en:

```text
storage/operations/{operationId}/{documentCode}/{guid}.{extension}
```

Los registros documentales sembrados son metadata demo y no apuntan a PDFs físicos. Los archivos reales solo se descargan mediante un endpoint autenticado.

## Migraciones

La migración inicial está en `backend/Infrastructure/Migrations`.

```powershell
cd backend
dotnet ef database update
dotnet ef migrations add NombreDeLaMigracion --output-dir Infrastructure/Migrations
```

No se requiere instalar `dotnet-ef` en el entorno validado. Si no está disponible localmente:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

## Credenciales demo

- Correo: `admin@sace.local`
- Contraseña: `Admin123!`
- Rol: `Usuario`

La contraseña se almacena con PBKDF2-SHA256, salt aleatorio y 120,000 iteraciones; nunca en texto plano.

## Datos demo

- 12 operaciones de agosto y septiembre de 2026.
- Importaciones y exportaciones, operaciones concluidas y expedientes con hallazgos.
- Cinco tipos documentales, cinco reglas `DEMO-*`, auditorías, resultados, hallazgos e historial.
- Operación preparada para presentación: `IMP-2026-000001`, pedimento `26 48 1234 6000001`.
- Esa operación incluye Pedimento, Factura comercial y Documento IMMEX demo válidos, pero no Certificado de origen.

Flujo recomendado de demostración:

1. Iniciar sesión.
2. Abrir **Operaciones** → `IMP-2026-000001`.
3. Revisar Documentos, Auditoría y Hallazgos.
4. Abrir `HAL-*` del certificado de origen.
5. Pulsar **Iniciar corrección**.
6. Subir un PDF, XML o XLSX válido.
7. Ver la nueva versión, la re-auditoría automática y el hallazgo resuelto.
8. Consultar el timeline y el Historial general.

## Prueba con expedientes reales

La evolución documental permite probar expedientes con múltiples PDF, XML y XLSX sin incorporar los archivos reales al repositorio. Para reproducir el caso de importación:

1. Crear una operación desde **Operaciones → Nueva operación** y capturar el pedimento `26 16 3108 6002948`, fecha `19/07/2026`, periodo `2026-07`, clave `IN`, fracción `40169304`, Vietnam, México y aduana `160 - Manzanillo`.
2. Capturar factura `054926M`, COVE `COVE2687R4RU2`, valor `4358.25 USD`, cantidad `6500 Piezas` y peso bruto `126.8 kg`. Elegir IMMEX manualmente; el sistema no lo presupone.
3. Abrir el expediente y entrar a **Documentos → Cargar documentos**.
4. Arrastrar o seleccionar simultáneamente los PDF/XML/XLSX. SACE valida cada archivo, calcula SHA-256 y propone tipos mediante reglas transparentes.
5. Corregir la clasificación de archivos `CARTAS*` o de cualquier propuesta con baja confianza antes de continuar.
6. Revisar vacíos, XML malformados, formatos no soportados y posibles duplicados; por omisión los duplicados se omiten, pero puede elegirse guardarlos.
7. Cargar los documentos válidos, abrir sus categorías y revisar metadata, notas y referencias COVE, e-Document o UUID CFDI.
8. Ejecutar la auditoría y distinguir resultados `DEMO-*` de controles técnicos `TECH-*`.

Las reglas DEMO no constituyen una validación jurídica de cumplimiento aduanero. Los controles TECH validan características técnicas/documentales y tampoco interpretan obligaciones legales.

### Carga masiva y clasificación

- `POST /api/operations/{id}/documents/analyze` analiza una selección sin almacenar contenido.
- `POST /api/operations/{id}/documents/bulk` procesa cada archivo de forma independiente y devuelve aceptados, rechazados y duplicados omitidos.
- Los patrones `PS01`, `FC01`, `PL01`, `TR01`, `COVE01`, `QR01` y `DIG_ACUSE` generan propuestas editables.
- Los archivos `CARTAS01`, `CARTAS02`, etc. requieren selección manual.
- Los XML reconocidos pueden extraer UUID CFDI, COVE o e-Document; no se valida fiscalmente ante SAT ni se consulta VUCEM.
- `.ERR` y `.209` permanecen explícitamente fuera de soporte.

## Estructura del proyecto

```text
SACE/
├── backend/
│   ├── Contracts/
│   ├── Domain/
│   ├── Infrastructure/
│   │   └── Migrations/
│   ├── Services/
│   └── Program.cs
├── backend.Tests/
├── frontend/
│   └── src/
│       ├── lib/
│       └── pages/
├── storage/
├── docs/
├── SACE.sln
└── README.md
```

## Principales endpoints

- `POST /api/auth/login`
- `GET /api/dashboard`
- `GET|POST /api/operations`
- `GET /api/operations/{id}`
- `POST /api/operations/{id}/documents`
- `POST /api/operations/{id}/documents/analyze`
- `POST /api/operations/{id}/documents/bulk`
- `GET|PATCH /api/operations/{operationId}/documents/{documentId}`
- `GET /api/operations/{id}/requirements`
- `POST /api/operations/{id}/audit`
- `POST /api/operations/{id}/legal-audit`
- `GET /api/operations/{id}/legal-audits`
- `GET|POST /api/operations/{id}/adjustments`
- `POST /api/audit-results/{id}/manual-review`
- `GET /api/operations/{operationId}/documents/{documentId}/download`
- `GET /api/audits`
- `GET|POST /api/audits/monthly`
- `GET /api/findings`
- `GET /api/findings/{id}`
- `PATCH /api/findings/{id}/status`
- `POST /api/findings/{id}/correction`
- `GET|POST /api/normative-rules`
- `PATCH /api/normative-rules/{id}/toggle`
- `GET /api/audit-log`
- `GET /api/document-types`

Salvo login y health, todos requieren `Authorization: Bearer {token}`. Los errores de negocio usan un formato uniforme con `status`, `code`, `message` y `errors`.

## Validación y seguridad del MVP

- JWT con vigencia de ocho horas y endpoints protegidos.
- Validación backend de campos obligatorios y transiciones de hallazgos.
- Límite configurable de 10 MB por archivo.
- Lista permitida PDF/XLSX/XML, verificación de MIME, extensión, tamaño y firma/estructura básica.
- Nombres físicos aleatorios, normalización de rutas y protección contra path traversal.
- Historial inmutable de versiones: una corrección nunca reemplaza el archivo anterior.
- Logs sin contraseña, token ni rutas físicas.
- CORS limitado al frontend local en desarrollo.

## Pruebas

```powershell
dotnet restore SACE.sln
dotnet build SACE.sln --no-restore
dotnet test SACE.sln --no-build --no-restore

cd frontend
npm install
npm run build
npm run lint
```

La suite cubre el flujo existente y 21 casos del motor LEGAL: vigencias, exclusión DEMO/TECH, pedimento, factura, COVE, evidencia insuficiente, origen, RRNA, incrementables, hallazgos y revisión humana.

## Limitaciones actuales del MVP

- El corpus LEGAL v1 es limitado y requiere validación especializada antes de uso productivo; la auditoría mensual continúa siendo DEMO.
- El XML solo se valida sintácticamente; no existe parser oficial SAT/VUCEM.
- PDF se valida por firma y XLSX por estructura ZIP mínima; no hay OCR ni extracción avanzada.
- SQLite y almacenamiento local son adecuados para demostración, no para despliegue distribuido.
- Existe un solo rol y no hay multiempresa.
- No hay integración con SAT, ANAM, VUCEM, agentes aduanales ni servicios de notificación.
- Los datos documentales precargados no incluyen archivos binarios descargables.
- No se realiza OCR ni separación automática de PDFs multipágina.
- La extracción XML reconoce únicamente estructuras e identificadores confiables; XML genéricos permanecen sin clasificar.

## Próximos pasos

1. Validar y versionar reglas reales con especialistas de comercio exterior.
2. Incorporar multiempresa, roles y permisos granulares.
3. Sustituir almacenamiento local por Blob/S3 con antivirus y cuarentena.
4. Añadir extracción documental/OCR y un parser oficial de pedimentos.
5. Integrar fuentes autorizadas como VUCEM y procesos del agente aduanal.
6. Añadir notificaciones, observabilidad y despliegue productivo.
