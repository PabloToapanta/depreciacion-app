-- ============================================================
-- Depreciacion App - Modelo de datos (feature/modelo-datos)
-- Crea AuthDB y AssetsDB con sus tablas y datos semilla.
-- Idempotente: se puede ejecutar varias veces sin romper nada.
-- ============================================================

-- ============================================================
-- AUTHDB (base del AuthService)
-- ============================================================
IF DB_ID('AuthDB') IS NULL
    CREATE DATABASE AuthDB;
GO

USE AuthDB;
GO

IF OBJECT_ID('dbo.Usuarios') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        id              INT IDENTITY(1,1) PRIMARY KEY,
        usuario         NVARCHAR(50)  NOT NULL UNIQUE,
        password_hash   NVARCHAR(100) NOT NULL,
        nombre_completo NVARCHAR(100) NULL,
        fecha_creacion  DATETIME2     NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Seed de usuarios (hash BCrypt generado con BCrypt.Net-Next, workFactor 12)
-- admin  -> Admin123!
-- leslie -> Leslie123!
INSERT INTO dbo.Usuarios (usuario, password_hash, nombre_completo)
SELECT 'admin',  '$2a$12$QzbbQbPhGG1CIFMYOpOn7OLD.AigTsVERJPmsHeUBg.T.fh2UW2iK', 'Administrador'
WHERE NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE usuario = 'admin');
GO

INSERT INTO dbo.Usuarios (usuario, password_hash, nombre_completo)
SELECT 'leslie', '$2a$12$tzUhcxPxUOXAIGIJl0HYIeJlllHps4QWP.THeJaJJ38eRz0esIMly', 'Leslie'
WHERE NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE usuario = 'leslie');
GO

-- ============================================================
-- ASSETSDB (base del AssetsService)
-- Master-detail: Categorias (maestro) -> Activos (detalle)
-- ============================================================
IF DB_ID('AssetsDB') IS NULL
    CREATE DATABASE AssetsDB;
GO

USE AssetsDB;
GO

IF OBJECT_ID('dbo.Categorias') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias (
        id              INT IDENTITY(1,1) PRIMARY KEY,
        nombre          NVARCHAR(50) NOT NULL UNIQUE,
        vida_util_anios INT          NOT NULL
    );
END
GO

-- Seed de categorias: los 4 tipos validos de AGENTS.md (regla de negocio centralizada)
INSERT INTO dbo.Categorias (nombre, vida_util_anios)
SELECT nombre, vida_util_anios
FROM (VALUES
    ('Tecnología', 3),
    ('Vehículos',  5),
    ('Edificios',  20),
    ('Muebles',    3)
) AS seed(nombre, vida_util_anios)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Categorias);
GO

IF OBJECT_ID('dbo.Activos') IS NULL
BEGIN
    CREATE TABLE dbo.Activos (
        id             INT IDENTITY(1,1) PRIMARY KEY,
        categoria_id   INT           NOT NULL,
        nombre         NVARCHAR(100) NOT NULL,
        precio_compra  DECIMAL(10,2) NOT NULL,
        fecha_compra   DATE          NOT NULL,
        fecha_corte    DATE          NOT NULL,  -- obligatoria (AGENTS.md)
        fecha_registro DATETIME2     NOT NULL DEFAULT GETDATE(),
        -- NO ACTION (default de SQL Server) = RESTRICT:
        -- no se puede borrar una categoria que tenga activos
        CONSTRAINT FK_Activos_Categorias FOREIGN KEY (categoria_id)
            REFERENCES dbo.Categorias(id)
    );
END
GO

-- Indice para acelerar las busquedas por categoria
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Activos_Categoria')
    CREATE INDEX IX_Activos_Categoria ON dbo.Activos(categoria_id);
GO

-- ============================================================
-- USUARIOS DE ACCESO POR SERVICIO (principio de menor privilegio)
-- Cada servicio se conecta con su PROPIO login de SQL Server,
-- con acceso SOLO a su base. Nunca se usa 'sa' en las
-- connection strings de los servicios.
--   auth_user   -> solo AuthDB    (AuthService)
--   assets_user -> solo AssetsDB  (AssetsService)
-- Permisos: db_datareader + db_datawriter (leer/escribir datos;
-- las tablas ya existen, los servicios no necesitan crearlas).
-- ============================================================

-- Login de AuthService
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'auth_user')
    CREATE LOGIN auth_user WITH PASSWORD = 'AuthUser123!';
GO

USE AuthDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'auth_user')
    CREATE USER auth_user FOR LOGIN auth_user;
GO

ALTER ROLE db_datareader ADD MEMBER auth_user;
ALTER ROLE db_datawriter ADD MEMBER auth_user;
GO

-- Login de AssetsService
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'assets_user')
    CREATE LOGIN assets_user WITH PASSWORD = 'AssetsUser123!';
GO

USE AssetsDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'assets_user')
    CREATE USER assets_user FOR LOGIN assets_user;
GO

ALTER ROLE db_datareader ADD MEMBER assets_user;
ALTER ROLE db_datawriter ADD MEMBER assets_user;
GO