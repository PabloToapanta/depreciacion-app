# Registro de sesiones

Bitácora de trabajo del proyecto. La actualiza el coordinador al cierre de cada sesión.

## Sesión 1 — Día 1: Setup y arquitectura (18/09/2026, escribió Pablo)

### Configuración inicial
- Revisión del repo: se corrigió `AGENTS.mds` → `AGENTS.md` (el sistema no cargaba las reglas de negocio).
- Se agregó el flujo de trabajo Git Flow simplificado a `AGENTS.md`.
- Commit de configuración: `AGENTS.md`, `TASKS.md`, `opencode.json`, `.opencode/agent/coordinator.md`.

### Repositorio en GitHub
- Repo `PabloToapanta/depreciacion-app` creado.
- Se pusheó `develop` (NO `main`). La plantilla de GitHub sugería `git branch -M main`; se descartó porque el Git Flow del proyecto reserva `main` para el día 5 (vía `release/v1.0`).
- Leslie invitada como colaboradora.
- Credenciales HTTPS configuradas con token personal (se revocará al terminar el proyecto).

### Modelo de datos
- Se definió y aprobó el modelo master-detail (recomendación del docente):
  - `AuthDB`: tabla `Usuarios` (id, usuario, password_hash, nombre_completo, fecha_creacion).
  - `AssetsDB`: `Categorias` (id, nombre, vida_util_anios) + `Activos` (id, categoria_id FK, nombre, precio_compra, fecha_compra, fecha_corte, fecha_registro).
- Decisiones tomadas:
  - La fecha de corte se guarda en el activo al crearlo (no se pasa como parámetro al calcular).
  - Seed de usuarios con hash BCrypt pre-generado (BCrypt.Net-Next, workFactor 12).
  - FK `NO ACTION` (equivale a RESTRICT): no se puede borrar una categoría con activos.
  - La vida útil vive en `Categorias`, no en cada activo (regla centralizada).
- Script `scripts/modelo-datos.sql` ejecutado y verificado: 4 categorías, 2 usuarios, 3 tablas.
- Feature `feature/modelo-datos` mergeada a `develop`. **Excepción documentada:** Leslie no revisó el PR por no estar disponible; revisará al volver.

### Usuarios de acceso por servicio
- Se crearon logins de SQL Server por servicio (principio de menor privilegio, sugerido por Pablo):
  - `auth_user` → solo `AuthDB` (contraseña `AuthUser123!`).
  - `assets_user` → solo `AssetsDB` (contraseña `AssetsUser123!`).
- Permisos: `db_datareader` + `db_datawriter` (las tablas ya existen; los servicios solo leen/escriben datos).
- Aislamiento verificado con pruebas: cada login no puede abrir la base del otro servicio.
- Feature `feature/usuarios-bd` mergeada a `develop` (misma excepción de revisión).

### Reglas del agente actualizadas
- Dos bases de datos separadas (`AuthDB`, `AssetsDB`), no compartida.
- Fecha de corte obligatoria.
- Nombres reales: Leslie y Pablo (reemplazan persona A/B).
- Verificación de integración de Git Flow en cada sesión (rama correcta, nunca push a `main`, pull antes de feature branch, PRs revisadas, borrar ramas muertas).
- Retroalimentación de malas prácticas (punto 9 del agente).
- Commits atómicos (un commit = un cambio lógico).
- Carpeta `Docs/` con esta documentación.

### Pendientes
- Día 2 — Login con JWT:
  - Pablo: endpoint de login en AuthService + generación/firma de JWT.
  - Leslie: tutorial de React (useState, fetch) + pantalla de login que consume AuthService.

## Sesión 2 — Día 2: Login con JWT, parte backend (19/09/2026, escribió Pablo)

### Lo que se hizo
- Feature branch `feature/auth-login` (Pablo, backend).
- Infraestructura de datos en AuthService:
  - Paquetes NuGet: `Microsoft.EntityFrameworkCore.SqlServer` 8.0.11 y
    `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11 (versión 8.x porque el
    proyecto apunta a net8.0; la 10.x requiere .NET 10).
  - Modelo `Usuario` (mapea tabla `Usuarios` con DataAnnotations, ~ el mapeo que en
    JDBC se hace a mano con ResultSet).
  - `AuthDbContext` (DbContext con `DbSet<Usuario>`, ~ Connection + Statement de JDBC
    pero devolviendo objetos en vez de ResultSet).
  - Connection string en `appsettings.json` con el login `auth_user` (no `sa`).
  - Configuración JWT en `appsettings.json`: Key, Issuer, Audience, ExpireMinutes.
- DTOs: `LoginRequest` y `LoginResponse`.
- `AuthController` con `POST /api/auth/login`:
  - Busca el usuario por nombre (LINQ), verifica el hash con `BCrypt.Verify`.
  - Credenciales inválidas → 401 con mensaje genérico (no revela si el usuario existe).
  - Credenciales válidas → 200 con JWT firmado.
- `JwtService`: genera y firma el token (HS256, clave secreta de appsettings).

### Pruebas realizadas (curl contra http://localhost:5001)
- `admin` / `Admin123!` → **200** con token JWT (header/payload/firma verificados).
- `admin` / clave incorrecta → **401**.
- usuario inexistente → **401**.

### Decisiones / notas
- El JWT se genera en un servicio separado (`JwtService`) para mantener el controller
  limpio y reutilizar la lógica el día 4 (validación en AssetsService).
- El mensaje de error es el mismo para usuario inexistente y clave mala (seguridad:
  no dar pistas de qué usuarios existen).
- `feature/auth-login` mergeada a `develop`. **Excepción documentada:** Leslie no
  revisó el PR por no estar disponible; revisará al volver.

### Pendientes
- Día 2 — parte frontend (Leslie): tutorial de React + pantalla de login.
- Día 3 — Pablo: endpoints de activos y depreciación en AssetsService.

## Sesión 3 — Día 3: Lógica de depreciación + formulario, parte backend (19/09/2026, escribió Pablo)

### Lo que se hizo
- Feature branch `feature/assets-calculo-depreciacion` (Pablo, backend).
- Infraestructura de datos en AssetsService:
  - Paquete `Microsoft.EntityFrameworkCore.SqlServer` 8.0.11.
  - Modelos `Activo` y `Categoria` (mapean tablas de AssetsDB; `Activo` tiene
    propiedad de navegación a `Categoria` ~ JOIN con objetos).
  - `AssetsDbContext` con `DbSet<Activo>` y `DbSet<Categoria>`.
  - Connection string con el login `assets_user` (no `sa`).
- DTOs: `ActivoRequest`, `ActivoResponse`, `FilaDepreciacion`, `TablaDepreciacionResponse`.
- `DepreciacionService`: lógica pura de cálculo con las fórmulas de AGENTS.md
  (VD = precio − precio×0.10; VDA = VD / vida_útil; VDM = VDA / 12). Primera fila
  del mes de compra con dep=0; se detiene en la fecha de corte o al fin de vida útil.
- `ActivosController` (documentado en Swagger):
  - `POST /api/activos` — crea activo (valida categoría → 400 si no existe).
  - `GET /api/activos` — lista activos.
  - `GET /api/activos/{id}` — un activo.
  - `GET /api/activos/{id}/depreciacion` — tabla mensual (404 si no existe).

### Pruebas realizadas (curl contra http://localhost:5002)
- Tecnología $900, 01/01/2023, corte 01/01/2026 → **VDM=22.5**, 37 filas,
  última fila 01/01/2026 con acumulado=810 y real=90.00 (ejemplo de AGENTS.md ✅).
- Vehículos $10000, 01/01/2024, corte 01/01/2027 → VDM=150, se detiene en la
  fecha de corte (acumulado=5400, real=4600) antes del fin de vida útil de 5 años ✅.
- Categoría inexistente → 400 ✅.

### Decisiones / notas
- `DepreciacionService` separado del controller (lógica pura, fácil de probar).
- `decimal` (sufijo `m`) para montos de dinero, no `double`.
- Método `Mapear()` privado para evitar código duplicado en los 3 endpoints.
- `feature/assets-calculo-depreciacion` mergeada a `develop`. **Excepción
  documentada:** Leslie no revisó el PR por no estar disponible; revisará al volver.

### Pendientes
- Día 3 — parte frontend (Leslie): formulario de activo + tabla en React.
- Día 4 — ambos: JWT en header desde React, validación en AssetsService, PDF, CORS.

## Sesión 4 — Día 4: JWT en AssetsService + CORS, parte backend (19/09/2026, escribió Pablo)

### Lo que se hizo
- Feature branch `feature/assets-jwt` (Pablo, backend).
- AssetsService ahora valida el JWT antes de responder:
  - Paquete `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11.
  - Sección `Jwt` en `appsettings.json` con la MISMA clave/issuer/audience que
    AuthService (así confía en los tokens que AuthService emite).
  - `AddAuthentication().AddJwtBearer(...)` en `Program.cs` (valida firma,
    issuer, audience y expiración) + `app.UseAuthentication()` antes de
    `UseAuthorization()`.
  - `[Authorize]` en `ActivosController` (protege los 4 endpoints).
- CORS en AMBOS servicios: política `PermitirFrontend` que permite el origen
  `http://localhost:5173` (Vite) con cualquier header y método.

### Pruebas realizadas (curl)
- Sin token → 401 ✅
- Token inválido → 401 ✅
- Token real de AuthService (login admin/Admin123!) → 200 ✅
- CORS: OPTIONS con Origin `http://localhost:5173` → responde
  `Access-Control-Allow-Origin: http://localhost:5173` en ambos servicios ✅
- CORS: Origin `http://evil.com` → sin cabecera CORS (el navegador lo bloquea) ✅

### Decisiones / notas
- El CORS de AuthService se commiteó dentro de `feature/assets-jwt` porque es
  parte de la misma tarea "Resolver CORS entre servicios" (ambos servicios).
- Falta la prueba final de CORS desde React (cuando Leslie tenga el frontend).
- El middleware JwtBearer es el "validador automático": sin token válido
  responde 401 sin ejecutar el código del controller.

### Pendientes
- Día 3 — parte frontend (Leslie): formulario de activo + tabla en React.
- Día 4 — ambos: JWT en header desde React, botón de PDF, prueba final de CORS.