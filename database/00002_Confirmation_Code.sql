CREATE TABLE [dbo].[T_User_Confirmation_Code](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_User_Confirmation_Code_Id DEFAULT NEWID() NOT NULL,
	[UserId] [UNIQUEIDENTIFIER] NOT NULL,
	[ConfirmationCode] [VARCHAR](20) NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_User_Confirmation_Code_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_User_Confirmation_Code_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [dbo].[T_User] ADD [IsActive] BIT CONSTRAINT DF_T_User_Is_Active DEFAULT(0) NOT NULL
GO
