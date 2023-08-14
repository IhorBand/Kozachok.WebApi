CREATE TABLE dbo.T_Chat_Connections (
	Id uniqueidentifier DEFAULT newid() NOT NULL,
	UserId uniqueidentifier NOT NULL,
	RoomId UNIQUEIDENTIFIER NOT NULL,
	ConnectionId NVARCHAR(500) NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Chat_Connections_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Chat_Connections_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

ALTER TABLE dbo.T_Chat_Connections
ADD CONSTRAINT FK_T_Chat_Connections_UserId_To_T_User_Id 
FOREIGN KEY (UserId) 
REFERENCES dbo.T_User(Id);
GO

ALTER TABLE dbo.T_Chat_Connections
ADD CONSTRAINT FK_T_Chat_Connections_RoomId_To_T_Room_Id 
FOREIGN KEY (RoomId) 
REFERENCES dbo.T_Room(Id);
GO
