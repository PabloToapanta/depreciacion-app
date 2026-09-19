# Modelo de datos

Dos bases de datos SQL Server **independientes**, una por servicio (no comparten base).
Script de creación: `scripts/modelo-datos.sql` (idempotente, se puede re-ejecutar).

## AuthDB (base del AuthService)

### Tabla `Usuarios`

| Columna | Tipo | Notas |
|---|---|---|
| `id` | INT IDENTITY | PK |
| `usuario` | NVARCHAR(50) | UNIQUE, NOT NULL |
| `password_hash` | NVARCHAR(100) | Hash BCrypt (BCrypt.Net-Next, workFactor 12) |
| `nombre_completo` | NVARCHAR(100) | NULL |
| `fecha_creacion` | DATETIME2 | DEFAULT GETDATE() |

**Seed:** `admin` (contraseña `Admin123!`) y `leslie` (contraseña `Leslie123!`).

## AssetsDB (base del AssetsService)

Master-detail: `Categorias` (maestro) → `Activos` (detalle).

### Tabla `Categorias`

| Columna | Tipo | Notas |
|---|---|---|
| `id` | INT IDENTITY | PK |
| `nombre` | NVARCHAR(50) | UNIQUE — Tecnología, Vehículos, Edificios, Muebles |
| `vida_util_anios` | INT | 3, 5, 20, 3 |

La vida útil vive en la categoría, **no** en el activo: si cambia la regla de negocio, se
actualiza un solo registro y todos los activos existentes se recalculan con la nueva regla.

### Tabla `Activos`

| Columna | Tipo | Notas |
|---|---|---|
| `id` | INT IDENTITY | PK |
| `categoria_id` | INT | FK → `Categorias.id` (NO ACTION = RESTRICT) |
| `nombre` | NVARCHAR(100) | Ej. "Laptop Dell" |
| `precio_compra` | DECIMAL(10,2) | NOT NULL |
| `fecha_compra` | DATE | NOT NULL |
| `fecha_corte` | DATE | NOT NULL (obligatoria, se guarda al crear el activo) |
| `fecha_registro` | DATETIME2 | DEFAULT GETDATE() |

Índice `IX_Activos_Categoria` sobre `categoria_id`.

## Usuarios de acceso (logins de SQL Server)

Cada servicio se conecta con su propio login, con acceso solo a su base (principio de
menor privilegio). Nunca se usa `sa` en las connection strings.

| Login | Base | Permisos |
|---|---|---|
| `auth_user` | `AuthDB` | `db_datareader` + `db_datawriter` |
| `assets_user` | `AssetsDB` | `db_datareader` + `db_datawriter` |

Contraseñas (proyecto académico): `auth_user` → `AuthUser123!`, `assets_user` → `AssetsUser123!`.
En producción irían en variables de entorno o secretos, no en git.