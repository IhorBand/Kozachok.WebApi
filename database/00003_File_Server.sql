CREATE TABLE [dbo].[T_File_Server](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_File_Server_Id DEFAULT NEWID() NOT NULL,
	[Name] [VARCHAR](100) NOT NULL,
	[Path] [VARCHAR](MAX) NOT NULL,
	[Url] [VARCHAR](MAX) NOT NULL,
	[IsActive] [BIT] CONSTRAINT DF_T_File_Server_IsActive DEFAULT(0) NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_File_Server_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_File_Server_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

CREATE TABLE [dbo].[L_File_Type](
	[Id] TINYINT NOT NULL,
	[Value] NVARCHAR(200) NOT NULL,
	CONSTRAINT [L_File_Type_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

CREATE TABLE [dbo].[T_File](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_File_Id DEFAULT NEWID() NOT NULL,
	[Name] [VARCHAR](500) NOT NULL,
	[FileServerId] [UNIQUEIDENTIFIER] NOT NULL CONSTRAINT FK_T_File_FileServerId_To_T_File_Server_Id FOREIGN KEY REFERENCES [dbo].[T_File_Server](Id),
	[FileTypeId] [TINYINT] NOT NULL CONSTRAINT FK_T_File_FileTypeId_To_L_File_Type_Id FOREIGN KEY REFERENCES [dbo].[L_File_Type](Id),
	[Size] [BIGINT] NULL,
	[Extension] [Varchar](100) NULL,
	[FullPath] [VARCHAR](MAX) NOT NULL,
	[Url] [VARCHAR](MAX) NOT NULL,
	[IsAcknowledged] [BIT] NOT NULL CONSTRAINT DF_T_File_IsAcknowledged DEFAULT(0),
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_File_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_File_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [dbo].[T_User] ADD [ThumbnailImageFileId] [UNIQUEIDENTIFIER] NULL CONSTRAINT FK_T_User_ThumbnailImageFileId_To_T_File_Id FOREIGN KEY REFERENCES [dbo].[T_File](Id)
GO

INSERT INTO [dbo].[L_File_Type](Id, [Value])
VALUES(0, 'Other'),
	(1, 'Image'),
	(2, 'Text'),
	(3, 'Docx'),
	(4, 'Video')
GO
