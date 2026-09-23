# Nuevo microservicio: TablaConfiguracionService

Documentación de lo realizado para el microservicio `TablaConfiguracion` y su
integración con el frontend.

## Qué hace

Expone la configuración del sistema (nombre del sistema, nombres y apellidos de los
desarrolladores, versión y fecha) para que el frontend la muestre en la parte inferior,
en la zona de tablas.

Datos que muestra (definidos en la base de datos):

| Campo             | Valor                            |
| ----------------- | -------------------------------- |
| Nombre del sistema | Depreciación App                |
| Nombres y apellidos | Leslie Coello y Pablo Toapanta |
| Versión            | 2.0                             |
| Fecha              | la fecha de hoy (GETDATE())     |

## Arquitectura (Onion, como los otros dos servicios)

4 proyectos .NET independientes dentro de `backend/TablaConfiguracion/`:

```
TablaConfiguracion.Domain/         <- SIN dependencias de nadie
  Entities/Configuracion.cs          (mapea la tabla `configuracion`)

TablaConfiguracion.Application/   <- referencia SOLO a Domain
  Interfaces/IConfiguracionRepository.cs
  DTOs/ConfiguracionResponse.cs
  UseCases/ObtenerConfiguracionUseCase.cs

TablaConfiguracion.Infrastructure/ <- referencia a Application + Domain
  Persistence/TablaConfiguracionDbContext.cs
  Persistence/ConfiguracionRepository.cs

TablaConfiguracion.Api/           <- referencia a los tres
  Controllers/ConfiguracionController.cs
  Program.cs                       (DI + CORS + validacion JWT)
  appsettings.json                 (connection string ConfigDB + JWT)
```

Regla de dependencia verificada con `dotnet list <proyecto> reference`:

- Domain: sin referencias a proyectos.
- Application: solo referencia a `TablaConfiguracion.Domain`.
- Infrastructure: referencia a `TablaConfiguracion.Application` y `.Domain`.
- Api: referencia a los tres.

## Endpoint

`GET http://localhost:5003/api/configuracion` (con `Authorization: Bearer <token>`)

Respuesta (JSON):

```json
{
  "nombreSistema": "Depreciación App",
  "nombresApellidos": "Leslie Coello y Pablo Toapanta",
  "version": "2.0",
  "fecha": "2026-09-23T00:00:00"
}
```

El servicio corre en el **puerto 5003** (`launchSettings.json`), valida el JWT emitido
por AuthService (misma clave, issuer y audience) y tiene CORS habilitado para el
frontend de Vite (`http://localhost:5173`).

## Base de datos (ConfigDB)

Nueva base `ConfigDB` con la tabla `configuracion`:

| Columna            | Tipo          | Descripción                                 |
| ------------------ | ------------- | ------------------------------------------- |
| id                 | INT IDENTITY  | Primary key                                 |
| nombre_sistema     | NVARCHAR(100) | Nombre del sistema                          |
| nombres_apellidos  | NVARCHAR(200) | Leslie Coello y Pablo Toapanta              |
| version            | NVARCHAR(20)  | 2.0                                         |
| fecha              | DATE          | DEFAULT CAST(GETDATE() AS DATE) = fecha hoy |

La tabla se crea con datos semilla si está vacía (idempotente).

Script actualizado: `scripts/modelo-datos.sql`. Incluye:

1. Creación de `ConfigDB` y de la tabla `dbo.configuracion`.
2. Seed con nombre `'Depreciación App'`, desarrolladores `'Leslie Coello y Pablo
   Toapanta'`, versión `'2.0'` y fecha = GETDATE().
3. Login SQL Server `config_user` con contraseña `ConfigUser123!` con permiso SOLO
   sobre `ConfigDB` (rol `db_datareader` + `db_datawriter`, menor privilegio como
   `auth_user` y `assets_user`).

## Frontend (React)

- `frontend/src/api.js`: se agregó `CONFIG_URL = 'http://localhost:5003'` y la función
  `obtenerConfiguracion()` que llama a `/api/configuracion` con el header `Authorization`.
- `frontend/src/components/TablaConfiguracion.jsx`: componente nuevo que muestra la
  tabla con los 4 campos del sistema.
- `frontend/src/App.jsx`: al cargar sesión se obtiene también la configuración (junto
  con categorías y activos) y se renderiza `TablaConfiguracion` en un footer, en la
  parte inferior de la pantalla.
- `frontend/src/App.css`: estilos del footer y de la tabla de configuración.

## Pasos seguidos

1. Se creó la rama `feature/tabla-configuracion` desde `develop`.
2. Se generaron los 4 proyectos con `dotnet new` y se cablearon las referencias
   respetando la regla de dependencia de Onion.
3. Se agregaron los paquetes `Microsoft.EntityFrameworkCore.SqlServer 8.0.11`
   (Infrastructure) y `Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11` +
   `Swashbuckle.AspNetCore 6.6.2` (Api). Versiones fijadas en 8.0.11 por
   compatibilidad con net8.0.
4. Se implementaron entidad, interfaz, DTO, use case, repositorio, DbContext,
   controller y `Program.cs`.
5. Se agregaron los 4 proyectos a `backend/DepreciacionApp.Backend.sln` dentro de la
   carpeta de solución `TablaConfiguracion`.
6. Se actualizó `scripts/modelo-datos.sql` (ConfigDB + tabla + seed + config_user).
7. Se integró el frontend (api.js, componente, App.jsx, App.css).
8. Se verificó: `dotnet build` (0 errores/0 warnings), `dotnet test` (4/4 pasan),
   `npm run lint` y `npm run build` (todo OK) y la regla de dependencia con
   `dotnet list ... reference`.

## Cómo probarlo

1. Ejecutar `scripts/modelo-datos.sql` en SQL Server (crea ConfigDB y su tabla).
2. Levantar los tres backends (AuthService :5001, AssetsService :5002,
   TablaConfiguracion :5003).
3. Levantar el frontend (`npm run dev`).
4. Iniciar sesión y ver la tabla "Información del sistema" al pie de la pantalla.