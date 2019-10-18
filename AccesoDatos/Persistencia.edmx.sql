
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/07/2019 21:41:24
-- Generated from EDMX file: C:\Users\Miguel\Dropbox\Tecnologías Proyecto\UvManServer\AccesoDatos\Persistencia.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DBUvMan];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CuentaUsuario]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CuentaSet] DROP CONSTRAINT [FK_CuentaUsuario];
GO
IF OBJECT_ID(N'[dbo].[FK_UsuarioAvance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UsuarioSet] DROP CONSTRAINT [FK_UsuarioAvance];
GO
IF OBJECT_ID(N'[dbo].[FK_AvancePersonajeCorredor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PersonajeCorredorSet] DROP CONSTRAINT [FK_AvancePersonajeCorredor];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CuentaSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CuentaSet];
GO
IF OBJECT_ID(N'[dbo].[UsuarioSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsuarioSet];
GO
IF OBJECT_ID(N'[dbo].[AvanceSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AvanceSet];
GO
IF OBJECT_ID(N'[dbo].[PersonajeCorredorSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PersonajeCorredorSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CuentaSet'
CREATE TABLE [dbo].[CuentaSet] (
    [Usuario] nvarchar(50)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [CodigoVerificacion] nvarchar(10)  NOT NULL,
    [Valida] bit  NOT NULL,
    [Usuario1_Id] int  NOT NULL
);
GO

-- Creating table 'UsuarioSet'
CREATE TABLE [dbo].[UsuarioSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Edad] nvarchar(3)  NOT NULL,
    [CorreoElectronico] nvarchar(max)  NOT NULL,
    [Avance_Id] int  NOT NULL
);
GO

-- Creating table 'AvanceSet'
CREATE TABLE [dbo].[AvanceSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UvCoins] int  NOT NULL,
    [MejorPuntuacion] int  NOT NULL
);
GO

-- Creating table 'PersonajeCorredorSet'
CREATE TABLE [dbo].[PersonajeCorredorSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nombre] nvarchar(50)  NOT NULL,
    [Precio] int  NOT NULL,
    [Poder] nvarchar(50)  NOT NULL,
    [AvanceId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Usuario] in table 'CuentaSet'
ALTER TABLE [dbo].[CuentaSet]
ADD CONSTRAINT [PK_CuentaSet]
    PRIMARY KEY CLUSTERED ([Usuario] ASC);
GO

-- Creating primary key on [Id] in table 'UsuarioSet'
ALTER TABLE [dbo].[UsuarioSet]
ADD CONSTRAINT [PK_UsuarioSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AvanceSet'
ALTER TABLE [dbo].[AvanceSet]
ADD CONSTRAINT [PK_AvanceSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PersonajeCorredorSet'
ALTER TABLE [dbo].[PersonajeCorredorSet]
ADD CONSTRAINT [PK_PersonajeCorredorSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Usuario1_Id] in table 'CuentaSet'
ALTER TABLE [dbo].[CuentaSet]
ADD CONSTRAINT [FK_CuentaUsuario]
    FOREIGN KEY ([Usuario1_Id])
    REFERENCES [dbo].[UsuarioSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaUsuario'
CREATE INDEX [IX_FK_CuentaUsuario]
ON [dbo].[CuentaSet]
    ([Usuario1_Id]);
GO

-- Creating foreign key on [Avance_Id] in table 'UsuarioSet'
ALTER TABLE [dbo].[UsuarioSet]
ADD CONSTRAINT [FK_UsuarioAvance]
    FOREIGN KEY ([Avance_Id])
    REFERENCES [dbo].[AvanceSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsuarioAvance'
CREATE INDEX [IX_FK_UsuarioAvance]
ON [dbo].[UsuarioSet]
    ([Avance_Id]);
GO

-- Creating foreign key on [AvanceId] in table 'PersonajeCorredorSet'
ALTER TABLE [dbo].[PersonajeCorredorSet]
ADD CONSTRAINT [FK_AvancePersonajeCorredor]
    FOREIGN KEY ([AvanceId])
    REFERENCES [dbo].[AvanceSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AvancePersonajeCorredor'
CREATE INDEX [IX_FK_AvancePersonajeCorredor]
ON [dbo].[PersonajeCorredorSet]
    ([AvanceId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------