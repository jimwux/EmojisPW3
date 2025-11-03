-- 0) Crear base de datos (ajustá el nombre si querés)
IF DB_ID('PW3_Emoji') IS NULL
    CREATE DATABASE PW3_Emoji;
GO
USE PW3_Emoji;
GO

/* 1) Tablas base */

-- Rol
IF OBJECT_ID('dbo.Rol', 'U') IS NULL
CREATE TABLE dbo.Rol (
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre        NVARCHAR(50) NOT NULL
);
GO

-- Usuario
IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
CREATE TABLE dbo.Usuario (
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Email         NVARCHAR(256) NOT NULL,
    HashPassword  NVARCHAR(256) NOT NULL,
    Nombre        NVARCHAR(100) NOT NULL,
    RolId         INT NOT NULL,
    CONSTRAINT UQ_Usuario_Email UNIQUE (Email),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (RolId) REFERENCES dbo.Rol(Id)
);
GO

-- Imagen
IF OBJECT_ID('dbo.Imagen', 'U') IS NULL
CREATE TABLE dbo.Imagen (
    Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UsuarioId    INT NOT NULL,
    Ruta         NVARCHAR(400) NOT NULL,  -- ej: /uploads/abc.jpg
    FechaSubida  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Ancho        INT NULL,
    Alto         INT NULL,
    CONSTRAINT FK_Imagen_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario(Id)
);
GO

-- Emocion
IF OBJECT_ID('dbo.Emocion', 'U') IS NULL
CREATE TABLE dbo.Emocion (
    Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre       NVARCHAR(50) NOT NULL,
    Descripcion  NVARCHAR(100) NULL,
    CONSTRAINT UQ_Emocion_Nombre UNIQUE (Nombre)
);
GO

-- AnalisisResultado (renombrado)
IF OBJECT_ID('dbo.AnalisisResultado', 'U') IS NULL
CREATE TABLE dbo.AnalisisResultado (
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ImagenId      INT NOT NULL,
    EmocionId     INT NOT NULL,
    Confianza     FLOAT NOT NULL,
    VectorJson    NVARCHAR(MAX) NULL,
    FechaAnalisis DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Analisis_Imagen  FOREIGN KEY (ImagenId)  REFERENCES dbo.Imagen(Id),
    CONSTRAINT FK_Analisis_Emocion FOREIGN KEY (EmocionId) REFERENCES dbo.Emocion(Id)
);
GO

/* 2) Índices */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Imagen_UsuarioId' AND object_id = OBJECT_ID('dbo.Imagen'))
    CREATE INDEX IX_Imagen_UsuarioId ON dbo.Imagen(UsuarioId);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Analisis_ImagenId' AND object_id = OBJECT_ID('dbo.AnalisisResultado'))
    CREATE INDEX IX_Analisis_ImagenId ON dbo.AnalisisResultado(ImagenId);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Analisis_EmocionId' AND object_id = OBJECT_ID('dbo.AnalisisResultado'))
    CREATE INDEX IX_Analisis_EmocionId ON dbo.AnalisisResultado(EmocionId);
GO

/* 3) Seed básico */

-- Roles
IF NOT EXISTS (SELECT 1 FROM dbo.Rol)
INSERT INTO dbo.Rol (Nombre) VALUES ('USUARIO'), ('ADMIN');

-- Emociones
IF NOT EXISTS (SELECT 1 FROM dbo.Emocion)
INSERT INTO dbo.Emocion (Nombre, Descripcion) VALUES
('happy',    N'Felicidad'),
('sad',      N'Tristeza'),
('angry',    N'Enojo'),
('fear',     N'Miedo'),
('surprise', N'Sorpresa'),
('neutral',  N'Neutro'),
('disgust',  N'Asco');

