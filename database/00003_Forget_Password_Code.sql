CREATE TABLE [dbo].[T_User_Forget_Password_Code](
	[Id] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_User_Forget_Password_Code_Id DEFAULT NEWID() NOT NULL,
	[UserId] [UNIQUEIDENTIFIER] NOT NULL,
	[ForgetPasswordCode] [VARCHAR](150) NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_User_Forget_Password_Code_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_User_Forget_Password_Code_Id] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)
GO
