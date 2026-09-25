# Despliegue del piloto SACE

Esta guía publica el frontend React/Vite en Vercel y la API ASP.NET Core 10 en Render usando el `Dockerfile` de la raíz. El piloto conserva SQLite y los documentos en un disco persistente de Render montado en `/app/data`.

## Arquitectura del piloto

- Vercel sirve el frontend estático y resuelve las rutas de la SPA mediante `frontend/vercel.json`.
- Render construye únicamente `backend/Sace.Api.csproj` y ejecuta `Sace.Api.dll` dentro del contenedor .NET 10.
- La API usa `/app/data/sace.db` para SQLite, `/app/data/storage` para documentos y `/app/data/keys` para las llaves de Data Protection.
- Render termina TLS. La aplicación escucha HTTP dentro del contenedor en el puerto que Render entrega mediante `PORT`.

## 1. Crear el proyecto de Vercel

1. Importe el repositorio en Vercel.
2. Defina **Root Directory** como `frontend`.
3. Seleccione **Framework Preset**: Vite.
4. Use **Build Command**: `npm run build`.
5. Use **Output Directory**: `dist`.
6. Cree inicialmente `VITE_API_URL` con una URL provisional o continúe sin probar el inicio de sesión hasta que Render entregue la URL definitiva.
7. Despliegue y anote el dominio HTTPS final de Vercel, sin `/` al final. Se utilizará como origen CORS en Render.

La variable definitiva debe ser:

```text
VITE_API_URL=https://NOMBRE-DEL-SERVICIO.onrender.com/api
```

Vite incorpora esta variable durante el build. Después de crearla o cambiarla se debe volver a desplegar el frontend.

## 2. Crear el Web Service de Render

1. Cree un **Web Service** desde el mismo repositorio.
2. Seleccione el runtime **Docker**.
3. Mantenga **Root Directory** en la raíz del repositorio.
4. Indique `Dockerfile` como ruta del Dockerfile. No configure un comando de inicio adicional: la imagen ya ejecuta `dotnet Sace.Api.dll`.
5. Configure **Health Check Path** como `/health`.
6. Agregue un disco persistente y móntelo exactamente en `/app/data`. Para un piloto pequeño, 1 GB es un punto de partida; ajuste su tamaño conforme crezcan la base y los documentos.
7. Cargue las variables indicadas abajo. Render proporciona `PORT` automáticamente; no es necesario fijarlo.

## Variables de Render

Variables obligatorias para el piloto:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Data Source=/app/data/sace.db
Storage__RootPath=/app/data/storage
DataProtection__KeysPath=/app/data/keys
Uploads__MaxFileSizeMb=25
Jwt__Key=<secreto aleatorio de al menos 32 caracteres>
Jwt__Issuer=SACE
Jwt__Audience=SACE.Client
Cors__AllowedOrigins__0=https://DOMINIO-REAL-DE-VERCEL
Seed__DemoData=false
Swagger__Enabled=false
PilotUser__Enabled=true
PilotUser__Email=<correo del responsable del piloto>
PilotUser__Password=<contraseña fuerte y exclusiva>
PilotUser__DisplayName=<nombre visible>
```

Para más de un origen permitido, agregue índices consecutivos:

```text
Cors__AllowedOrigins__1=https://OTRO-DOMINIO-AUTORIZADO
```

No use `*`, no agregue una diagonal final al origen y no reutilice la clave de desarrollo incluida en `appsettings.Development.json`. Mantenga `Jwt__Key` y `PilotUser__Password` como valores secretos en Render.

## Inicio, migraciones y semillas

En cada inicio la API:

1. crea los directorios padre de SQLite, almacenamiento y Data Protection si no existen;
2. ejecuta las migraciones pendientes con `Database.Migrate()`;
3. conserva todos los datos existentes: no elimina ni recrea la base;
4. inserta catálogos y reglas faltantes;
5. crea el usuario piloto solamente si `PilotUser__Enabled=true` y el correo todavía no existe;
6. omite las operaciones y documentos de demostración cuando `Seed__DemoData=false`.

La contraseña del usuario piloto se guarda con el mecanismo de hash ya utilizado por SACE. Cambiar `PilotUser__Password` después de que el usuario existe **no** reemplaza su contraseña ni modifica su cuenta.

## Persistencia y respaldo

El disco debe estar montado en `/app/data`; de otro modo una nueva imagen perderá SQLite, documentos y llaves de Data Protection. Las rutas de producción son:

- SQLite: `/app/data/sace.db`
- documentos: `/app/data/storage`
- llaves de Data Protection: `/app/data/keys`

Antes de una actualización importante, haga una copia coherente del disco. Detenga temporalmente las escrituras y respalde juntos `sace.db`, cualquier archivo `sace.db-wal`/`sace.db-shm` presente, `storage` y `keys`. Verifique periódicamente que el respaldo pueda restaurarse. Las copias y la retención se administran desde Render o mediante el procedimiento operativo elegido por la empresa; no están automatizadas por SACE.

SQLite sobre un único disco es apropiado para este piloto de baja concurrencia. No ejecute varias instancias de la API ni habilite escalamiento horizontal con esta arquitectura.

## Comprobación posterior al despliegue

1. Abra `https://NOMBRE-DEL-SERVICIO.onrender.com/health` y confirme exactamente `{"status":"healthy"}`.
2. Confirme en los logs de Render que las migraciones terminaron correctamente y que no aparecen secretos.
3. Abra el dominio de Vercel, inicie sesión con el usuario piloto y cree un expediente de prueba.
4. Cargue un PDF, XML y XLSX de prueba no sensible, vuelva a desplegar Render y confirme que el expediente y los archivos siguen disponibles.
5. Compruebe que `/swagger` no está disponible en Production.

## Operación segura del piloto

- No almacene documentos reales en Git ni en variables de entorno.
- No publique ni registre `Jwt__Key` o `PilotUser__Password`.
- Cambie las credenciales si estuvieron expuestas y use acceso limitado al panel de Render/Vercel.
- Mantenga `Seed__DemoData=false` en producción. Activarlo agrega datos DEMO y no debe usarse con información real.
- El piloto no incorpora alta disponibilidad, almacenamiento de objetos, OCR, IA ni integración externa con SAT/VUCEM.
