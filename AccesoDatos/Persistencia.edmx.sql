
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/15/2019 23:30:35
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
IF OBJECT_ID(N'[dbo].[FK_JugadorCorredoresAdquiridos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CorredorAdquiridoSet] DROP CONSTRAINT [FK_JugadorCorredoresAdquiridos];
GO
IF OBJECT_ID(N'[dbo].[FK_JugadorPerseguidorAdquirido]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PerseguidorAdquiridoSet] DROP CONSTRAINT [FK_JugadorPerseguidorAdquirido];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CuentaSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CuentaSet];
GO
IF OBJECT_ID(N'[dbo].[JugadorSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JugadorSet];
GO
IF OBJECT_ID(N'[dbo].[CorredorAdquiridoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CorredorAdquiridoSet];
GO
IF OBJECT_ID(N'[dbo].[PerseguidorAdquiridoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PerseguidorAdquiridoSet];
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
    [CorreoElectronico] nvarchar(max)  NOT NULL,
    [Usuario1_Id] int  NOT NULL
);
GO

-- Creating table 'JugadorSet'
CREATE TABLE [dbo].[JugadorSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MejorPuntacion] int  NOT NULL,
    [UvCoins] int  NOT NULL
);
GO

-- Creating table 'CorredorAdquiridoSet'
CREATE TABLE [dbo].[CorredorAdquiridoSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nombre] nvarchar(50)  NOT NULL,
    [Precio] int  NOT NULL,
    [Poder] nvarchar(50)  NOT NULL,
    [JugadorId] int  NOT NULL
);
GO

-- Creating table 'PerseguidorAdquiridoSet'
CREATE TABLE [dbo].[PerseguidorAdquiridoSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nombre] nvarchar(max)  NOT NULL,
    [Precio] int  NOT NULL,
    [JugadorId] int  NOT NULL
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

-- Creating primary key on [Id] in table 'JugadorSet'
ALTER TABLE [dbo].[JugadorSet]
ADD CONSTRAINT [PK_JugadorSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CorredorAdquiridoSet'
ALTER TABLE [dbo].[CorredorAdquiridoSet]
ADD CONSTRAINT [PK_CorredorAdquiridoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PerseguidorAdquiridoSet'
ALTER TABLE [dbo].[PerseguidorAdquiridoSet]
ADD CONSTRAINT [PK_PerseguidorAdquiridoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Usuario1_Id] in table 'CuentaSet'
ALTER TABLE [dbo].[CuentaSet]
ADD CONSTRAINT [FK_CuentaUsuario]
    FOREIGN KEY ([Usuario1_Id])
    REFERENCES [dbo].[JugadorSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaUsuario'
CREATE INDEX [IX_FK_CuentaUsuario]
ON [dbo].[CuentaSet]
    ([Usuario1_Id]);
GO

-- Creating foreign key on [JugadorId] in table 'CorredorAdquiridoSet'
ALTER TABLE [dbo].[CorredorAdquiridoSet]
ADD CONSTRAINT [FK_JugadorCorredoresAdquiridos]
    FOREIGN KEY ([JugadorId])
    REFERENCES [dbo].[JugadorSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JugadorCorredoresAdquiridos'
CREATE INDEX [IX_FK_JugadorCorredoresAdquiridos]
ON [dbo].[CorredorAdquiridoSet]
    ([JugadorId]);
GO

-- Creating foreign key on [JugadorId] in table 'PerseguidorAdquiridoSet'
ALTER TABLE [dbo].[PerseguidorAdquiridoSet]
ADD CONSTRAINT [FK_JugadorPerseguidorAdquirido]
    FOREIGN KEY ([JugadorId])
    REFERENCES [dbo].[JugadorSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JugadorPerseguidorAdquirido'
CREATE INDEX [IX_FK_JugadorPerseguidorAdquirido]
ON [dbo].[PerseguidorAdquiridoSet]
    ([JugadorId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------