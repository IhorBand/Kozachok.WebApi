CREATE TABLE [dbo].[L_Country] (
	[Id] int NOT NULL,
	[Name] nvarchar(200) NOT NULL,
	TranslationKey uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_L_Country_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT L_Country_Id PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
);
GO

CREATE TABLE dbo.L_Movie_Genre (
	Id int NOT NULL,
	[Value] nvarchar(200) NOT NULL,
	TranslationKey uniqueidentifier NULL,
	CONSTRAINT L_Movie_Genre_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.L_Movie_Main_Category (
	Id tinyint NOT NULL,
	[Value] nvarchar(200) NOT NULL,
	CONSTRAINT L_Movie_Main_Category_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.L_Movie_Type (
	Id tinyint NOT NULL,
	[Value] nvarchar(200) NOT NULL,
	CONSTRAINT L_Movie_Type_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.L_Translation_Language (
	Id int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(100) NULL,
	FullName varchar(100) NULL,
	IsoCode varchar(5) NULL,
	CreatedDateUTC datetime CONSTRAINT DF_L_Translation_Language_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT L_Translation_Language_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE [dbo].[L_Translator] (
	[Id] UNIQUEIDENTIFIER CONSTRAINT DF_L_Translator_Id DEFAULT newid() NOT NULL,
	[Name] nvarchar(400) NOT NULL,
	TranslatorID nvarchar(100) NULL,
	LanguageId INT NULL,
	CreatedDateUTC datetime CONSTRAINT DF_L_Translator_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT L_Translator_Id PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
);
GO

CREATE TABLE dbo.T_Movie (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Id DEFAULT newid() NOT NULL,
	VideoId varchar(200) NULL,
	VideoUrlId varchar(400) NULL,
	AdditionalCoverUrl varchar(400) NULL,
	ShortTitle nvarchar(300) NULL,
	FullTitle nvarchar(300) NULL,
	OriginalFullTitle nvarchar(500) NULL,
	ShortDescription nvarchar(300) NULL,
	FullDescription nvarchar(MAX) NULL,
	ImdbId varchar(200) NULL,
	ImdbUrl varchar(400) NULL,
	ImdbCoverUrl varchar(400) NULL,
	ImdbDescription nvarchar(MAX) NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	VideoFullUrl varchar(400) NULL,
	TypeId tinyint NULL,
	MainCategoryId tinyint NULL,
	ReleaseDate datetime NULL,
	CONSTRAINT T_Movie_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Actor (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Actor_Id DEFAULT newid() NOT NULL,
	FirstName nvarchar(200) NULL,
	LastName nvarchar(200) NULL,
	ImageUrl varchar(400) NULL,
	MovieId uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Actor_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Movie_Actor_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Country (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Country_Id DEFAULT newid() NOT NULL,
	MovieId uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Country_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CountryId int NULL,
	CONSTRAINT T_Movie_Country_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Director (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Director_Id DEFAULT newid() NOT NULL,
	FirstName nvarchar(200) NULL,
	LastName nvarchar(200) NULL,
	MovieId uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Director_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Movie_Rezka_Director_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Genre (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Genre_Id DEFAULT newid() NOT NULL,
	MovieId uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Genre_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	GenreId int NULL,
	CONSTRAINT T_Movie_Rezka_Genre_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Image (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Image_Id DEFAULT newid() NOT NULL,
	[Value] varchar(400) NULL,
	MovieId uniqueidentifier NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Image_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Movie_Image_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Movie_Translator (
	Id uniqueidentifier CONSTRAINT DF_T_Movie_Translator_Id DEFAULT newid() NOT NULL,
	MovieId uniqueidentifier NULL,
	TranslatorId UNIQUEIDENTIFIER NOT NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Movie_Translator_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Movie_Translator_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

CREATE TABLE dbo.T_Translation (
	Id uniqueidentifier CONSTRAINT DF_T_Translation_Id DEFAULT newid() NOT NULL,
	[Key] uniqueidentifier CONSTRAINT DF_T_Translation_Key DEFAULT newid() NOT NULL,
	TranslationLanguageId int NOT NULL,
	[Value] nvarchar(500) NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Translation_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Translation_Id PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
);
GO

ALTER TABLE dbo.T_Movie 
ADD CONSTRAINT FK_T_Movie_MainCategoryId_To_L_Movie_Main_Category_Id 
FOREIGN KEY (MainCategoryId) 
REFERENCES dbo.L_Movie_Main_Category(Id);
GO

ALTER TABLE dbo.T_Movie 
ADD CONSTRAINT FK_T_Movie_TypeId_To_L_Movie_Type_Id 
FOREIGN KEY (TypeId) 
REFERENCES dbo.L_Movie_Type(Id);
GO

ALTER TABLE dbo.T_Movie_Country 
ADD CONSTRAINT FK_T_Movie_Country_CountryId_To_L_Country_Id 
FOREIGN KEY (CountryId) 
REFERENCES dbo.L_Country(Id);
GO

ALTER TABLE dbo.T_Movie_Country 
ADD CONSTRAINT FK_T_Movie_Country_MovieId_To_T_Movie_Id 
FOREIGN KEY (MovieId) 
REFERENCES dbo.T_Movie(Id);
GO

ALTER TABLE dbo.T_Movie_Genre 
ADD CONSTRAINT FK_T_Movie_Genre_GenreId_To_L_Movie_Genre_Id 
FOREIGN KEY (GenreId) 
REFERENCES dbo.L_Movie_Genre(Id);
GO

ALTER TABLE dbo.T_Translation 
ADD CONSTRAINT FK_T_Translation_TranslationLanguageId_To_L_Translation_Language_Id 
FOREIGN KEY (TranslationLanguageId) 
REFERENCES dbo.L_Translation_Language(Id);
GO

ALTER TABLE [dbo].[L_Translator]
ADD CONSTRAINT FK_L_Translator_LanguageId_To_L_Translation_Language_Id 
FOREIGN KEY (LanguageId) 
REFERENCES dbo.L_Translation_Language(Id);
GO

ALTER TABLE dbo.T_Movie_Translator 
ADD CONSTRAINT FK_T_Movie_Translator_TranslatorId_To_L_Translator_Id 
FOREIGN KEY (TranslatorId) 
REFERENCES dbo.L_Translator(Id);
GO

ALTER TABLE dbo.T_Movie_Translator 
ADD CONSTRAINT FK_T_Movie_Translator_MovieId_To_T_Movie_Id 
FOREIGN KEY (MovieId) 
REFERENCES dbo.T_Movie(Id);
GO
