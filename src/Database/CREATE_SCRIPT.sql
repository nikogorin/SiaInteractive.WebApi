CREATE DATABASE [SiaInteractive]
GO

USE [SiaInteractive]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Product](
	[ProductID] [INT] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](200) NOT NULL,
	[Description] [NVARCHAR](MAX) NULL,
	[Image] [VARBINARY](MAX) NULL,
	CONSTRAINT PK_Product PRIMARY KEY ([ProductID]),
	CONSTRAINT UQ_Product_Name UNIQUE ([Name])
)
GO

CREATE TABLE [dbo].[Category](
	[CategoryID] [INT] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](200) NOT NULL,
	CONSTRAINT PK_Category PRIMARY KEY ([CategoryID]),
	CONSTRAINT UQ_Category_Name UNIQUE ([Name])
)
GO

CREATE TABLE [dbo].[ProductCategory](
	[ProductID] [INT] NOT NULL,
	[CategoryID] [INT] NOT NULL,
	CONSTRAINT PK_ProductCategory PRIMARY KEY ([ProductID], [CategoryID]),
	CONSTRAINT FK_ProductCategory_Product FOREIGN KEY (ProductID) REFERENCES [Product](ProductID) ON DELETE CASCADE,
	CONSTRAINT FK_ProductCategory_Category FOREIGN KEY (CategoryID) REFERENCES [Category](CategoryID) ON DELETE CASCADE
)
GO