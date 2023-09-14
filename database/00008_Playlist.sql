CREATE TABLE dbo.T_Playlist_Movie (
	Id uniqueidentifier DEFAULT newid() NOT NULL,
	MovieId uniqueidentifier NULL,
	RoomId UNIQUEIDENTIFIER NOT NULL,
	OrderNumber INT NOT NULL,
	VideoId VARCHAR(200) NULL,
	VideoUrlId VARCHAR(400) NULL,
	Name NVARCHAR(400) NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Playlist_Movie_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Playlist_Movie_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

ALTER TABLE dbo.T_Playlist_Movie 
ADD CONSTRAINT FK_T_Playlist_Movie_RoomId_To_T_Room_Id 
FOREIGN KEY (RoomId) 
REFERENCES dbo.T_Room(Id);
GO

ALTER TABLE dbo.T_Playlist_Movie 
ADD CONSTRAINT FK_T_Playlist_Movie_MovieId_To_T_Movie_Id 
FOREIGN KEY (MovieId) 
REFERENCES dbo.T_Movie(Id);
GO

CREATE TABLE dbo.T_Playlist_Movie_Video (
	Id UNIQUEIDENTIFIER DEFAULT newid() NOT NULL,
	PlaylistMovieId UNIQUEIDENTIFIER NULL,
	Season INT NOT NULL,
	Episode INT NOT NULL,
	TranslatorExternalId VARCHAR(100) NOT NULL,
	TranslatorName NVARCHAR(400),
	CreatedDateUTC DATETIME CONSTRAINT DF_T_Playlist_Movie_Video_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Playlist_Movie_Video_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

ALTER TABLE dbo.T_Playlist_Movie_Video
ADD CONSTRAINT FK_T_Playlist_Movie_Video_PlaylistMovieId_To_T_Playlist_Movie_Id 
FOREIGN KEY (PlaylistMovieId) 
REFERENCES dbo.T_Playlist_Movie(Id);
GO

CREATE TABLE dbo.T_Playlist_Movie_Video_Quality (
	Id uniqueidentifier DEFAULT newid() NOT NULL,
	PlaylistMovieId uniqueidentifier NOT NULL,
	PlaylistMovieVideoId UNIQUEIDENTIFIER NOT NULL,
	QualityId INT NOT NULL,
	MovieUrl VARCHAR(500) NOT NULL,
	CreatedDateUTC datetime CONSTRAINT DF_T_Playlist_Movie_Video_Quality_CreatedDateUTC DEFAULT getutcdate() NOT NULL,
	CONSTRAINT T_Playlist_Movie_Video_Quality_Id PRIMARY KEY CLUSTERED
	(
		Id ASC
	)
);
GO

ALTER TABLE dbo.T_Playlist_Movie_Video_Quality 
ADD CONSTRAINT FK_T_Playlist_Movie_Video_Quality_PlaylistMovieId_To_T_Playlist_Movie_Id 
FOREIGN KEY (PlaylistMovieId) 
REFERENCES dbo.T_Playlist_Movie(Id);
GO

ALTER TABLE dbo.T_Playlist_Movie_Video_Quality 
ADD CONSTRAINT FK_T_Playlist_Movie_Video_Quality_PlaylistMovieVideoId_To_T_Playlist_Movie_Video_Id 
FOREIGN KEY (PlaylistMovieVideoId) 
REFERENCES dbo.T_Playlist_Movie_Video(Id);
GO
