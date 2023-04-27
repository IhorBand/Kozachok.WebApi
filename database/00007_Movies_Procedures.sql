CREATE PROC [dbo].[InsertNewTranslator] 
	@MovieId [UNIQUEIDENTIFIER],
	@TranslatorName [NVARCHAR](400) NULL,
	@TranslatorId [VARCHAR](100) NULL,
	@TranslatorLanguage [NVARCHAR](300) NULL
AS
BEGIN
	INSERT INTO [dbo].[T_Movie_Translator_Bak](
		MovieId,
		TranslatorName,
		TranslatorId,
		TranslatorLanguage)
	OUTPUT Inserted.Id
	VALUES(
		@MovieId,
		@TranslatorName,
		@TranslatorId,
		@TranslatorLanguage
		);

	IF @TranslatorName IS NULL OR  @TranslatorName = '' OR (@TranslatorName IS NOT NULL AND @TranslatorName <> '' AND ISNUMERIC(@TranslatorId) = 0)
	BEGIN
		-- not available
		INSERT INTO T_Movie_Translator (MovieId, TranslatorId) VALUES(@MovieId, (SELECT TOP 1 lt.Id FROM L_Translator lt WHERE lt.TranslatorID = 'NULL'))
	END
	ELSE IF EXISTS (SELECT * FROM L_Translator lt WHERE lt.TranslatorID = @TranslatorID)
	BEGIN 
		-- available, need to link by id
		INSERT INTO T_Movie_Translator (MovieId, TranslatorId) VALUES(@MovieId, (SELECT TOP 1 lt.Id FROM L_Translator lt WHERE lt.TranslatorID = @TranslatorID))
	END
	ELSE
	BEGIN
		-- available, but need to add new record in L_Translator
		DECLARE @TranslatorLanguageId INT =
		(
			SELECT CASE 
				WHEN @TranslatorLanguage = N'Украинский' THEN 1
				WHEN @TranslatorLanguage = N'Казахский' THEN 4
				WHEN @TranslatorLanguage IS NULL THEN 3
				ELSE 3
				END
		)
		
		DECLARE @InsertedId table ( Id UNIQUEIDENTIFIER )
		
		INSERT INTO L_Translator([Name], [TranslatorId], [LanguageId]) 
		OUTPUT inserted.Id INTO @InsertedId
		VALUES(@TranslatorName, @TranslatorId, @TranslatorLanguageId)
		
		INSERT INTO T_Movie_Translator(MovieId, TranslatorId) VALUES(@MovieId, (SELECT TOP 1 Id FROM @InsertedId))
	END
	
END
GO

CREATE PROCEDURE sp_SearchMovies
	@SearchValue NVARCHAR(200) = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @GenreId INT = 0,
    @CountryId INT = 0,
    @TotalCount INT OUTPUT,
    @TotalPages INT OUTPUT
WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @sql NVARCHAR(MAX) = ''
   
	SET @TotalCount = 0
	SET @TotalPages = 1

	IF @PageIndex <= 0
	BEGIN
		SET @PageIndex = 1
	END

	IF @PageSize < 10
	BEGIN
		SET @PageSize = 10
	END
		
	SET @sql = '
		IF OBJECT_ID (''tempdb..#searchMovieQuery'') IS NOT NULL
			DROP TABLE #searchMovieQuery
	
		SELECT
			[movie].*
		INTO #searchMovieQuery
		FROM [T_Movie] movie
		WHERE (LOWER(FullTitle) LIKE ''%'' + @SearchValue + ''%'' 
			OR LOWER(OriginalFullTitle) LIKE ''%'' + @SearchValue + ''%'')'
	
	IF @GenreId > 0
	BEGIN
		SET @sql += 'AND EXISTS (SELECT Id FROM T_Movie_Genre mg WHERE mg.MovieId = movie.Id AND mg.GenreId = @GenreId)'
	END
	
	IF @CountryId > 0
	BEGIN
		SET @sql += 'AND EXISTS (SELECT Id FROM T_Movie_Country mc WHERE mc.MovieId = movie.Id AND mc.CountryId = @CountryId)'
	END
	
	SET @sql += '
	
		SELECT @TotalCount = COUNT(*) FROM #searchMovieQuery
	
		SELECT @TotalPages = CEILING(@TotalCount / CAST(@PageSize AS FLOAT))
	
		SELECT * 
		FROM #searchMovieQuery
		ORDER BY CreatedDateUTC
		OFFSET (@PageIndex - 1) * @PageSize ROWS	-- skip  rows
		FETCH NEXT @PageSize ROWS ONLY;				-- take 10 rows
	
		DROP TABLE #searchMovieQuery'
	
	EXECUTE sp_executesql @sql, 
		N'@SearchValue NVARCHAR(200), 
		@PageSize INT, 
		@PageIndex INT, 
		@GenreId INT,
		@CountryId INT,
		@TotalCount INT OUTPUT, 
		@TotalPages INT OUTPUT', 
		@SearchValue, 
		@PageSize, 
		@PageIndex,
		@GenreId,
		@CountryId,
		@TotalCount OUTPUT, 
		@TotalPages OUTPUT
		
	SELECT @TotalCount, @TotalPages
END
GO

CREATE PROCEDURE sp_SearchMovies_Top
	@SearchValue NVARCHAR(200) = NULL
WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON;

	SELECT TOP 5
		[movie].*
	FROM [T_Movie] movie
	WHERE LOWER([RezkaFullTitle]) LIKE '%' + @SearchValue + '%' 
		OR LOWER([RezkaOriginalFullTitle]) LIKE '%' + @SearchValue + '%'
	ORDER BY [CreatedDateUTC] DESC
END
GO