CREATE TABLE [dbo].[L_Confirmation_Code_Type](
	[Id] TINYINT NOT NULL,
	[Value] NVARCHAR(200) NOT NULL,
	CONSTRAINT [L_Confirmation_Code_Type_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

INSERT INTO [dbo].[L_Confirmation_Code_Type](Id, [Value])
VALUES(0, 'Other'),
	(1, 'Email Confirmation'),
	(2, 'Forgot Password'),
	(3, 'Change Email')
GO

CREATE TABLE [dbo].[T_User_Confirmation_Code](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_User_Confirmation_Code_Id DEFAULT NEWID() NOT NULL,
	[UserId] [UNIQUEIDENTIFIER] NOT NULL CONSTRAINT FK_T_User_Confirmation_Code_UserId_To_T_User_Id FOREIGN KEY REFERENCES [dbo].[T_User](Id),
	[ConfirmationCode] [VARCHAR](150) NOT NULL,
	[CodeType] [TINYINT] NOT NULL CONSTRAINT FK_T_User_Confirmation_Code_CodeType_To_L_Confirmation_Code_Type_Id FOREIGN KEY REFERENCES [dbo].[L_Confirmation_Code_Type](Id),
	[AdditionalData] [VARCHAR](500) NULL,
	[NumberOfAttempt] [TINYINT] NOT NULL CONSTRAINT DF_T_User_Confirmation_Code_NumberOfAttempt DEFAULT(1),
	[NextAttemptDate] [DATETIME] NOT NULL CONSTRAINT DF_T_User_Confirmation_Code_NextAttemptDate DEFAULT GETUTCDATE(),
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_User_Confirmation_Code_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_User_Confirmation_Code_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [dbo].[T_User] ADD [IsActive] BIT CONSTRAINT DF_T_User_Is_Active DEFAULT(0) NOT NULL
GO
