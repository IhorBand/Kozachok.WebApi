CREATE TABLE [dbo].[T_User_Forget_Password_Code](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_User_Forget_Password_Code_Id DEFAULT NEWID() NOT NULL,
	[UserId] [UNIQUEIDENTIFIER] NOT NULL CONSTRAINT FK_T_User_Forget_Password_Code_UserId_To_T_User_Id FOREIGN KEY REFERENCES [dbo].[T_User](Id),
	[ForgetPasswordCode] [VARCHAR](150) NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_User_Forget_Password_Code_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_User_Forget_Password_Code_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO
