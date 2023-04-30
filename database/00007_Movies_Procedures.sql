CREATE PROCEDURE sp_SearchMovies
	@SearchValue NVARCHAR(200) = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @GenreId INT = 0,
    @CountryId INT = 0,
    @MovieTypeId INT = -1,
    @MovieMainCategoryId INT = 0,
    @OrderId INT = 0,
    @OrderDirectionId INT = 0,
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
		WHERE 1=1 --TODO: Added this as hotfix for filters 
	'
	
	IF @SearchValue IS NOT NULL AND TRIM(@SearchValue) <> ''
	BEGIN
		SET @sql += '
			AND ((LOWER(FullTitle) LIKE ''%'' + @SearchValue + ''%'' 
			OR LOWER(OriginalFullTitle) LIKE ''%'' + @SearchValue + ''%''))'
	END	
	
	IF @GenreId > 0
	BEGIN
		SET @sql += ' AND EXISTS (SELECT Id FROM T_Movie_Genre mg WHERE mg.MovieId = movie.Id AND mg.GenreId = @GenreId)'
	END
	
	IF @CountryId > 0
	BEGIN
		SET @sql += ' AND EXISTS (SELECT Id FROM T_Movie_Country mc WHERE mc.MovieId = movie.Id AND mc.CountryId = @CountryId)'
	END
	
	IF @MovieTypeId >= 0 AND @MovieTypeId < 2
	BEGIN
		SET @sql += ' AND movie.TypeId = @MovieTypeId'
	END
	
	IF @MovieMainCategoryId > 0 AND @MovieMainCategoryId < 5
	BEGIN
		SET @sql += ' AND movie.MainCategoryId = @MovieMainCategoryId'
	END
	
	SET @sql += '
	
		SELECT @TotalCount = COUNT(*) FROM #searchMovieQuery
	
		SELECT @TotalPages = CEILING(@TotalCount / CAST(@PageSize AS FLOAT))
	
		SELECT * 
		FROM #searchMovieQuery
		'
	
	IF @OrderId = 0
	BEGIN
		SET @sql += 'ORDER BY CreatedDateUTC '
	END 
	ELSE IF @OrderId = 1
	BEGIN 
		SET @sql += 'ORDER BY FullTitle '
	END
	ELSE IF @OrderId = 2
	BEGIN 
		SET @sql += 'ORDER BY ReleaseDate '
	END
	
	IF @OrderDirectionId = 0
	BEGIN 
		SET @sql += 'DESC'
	END
	ELSE IF @OrderDirectionId = 1
	BEGIN 
		SET @sql += 'ASC'
	END
	
	SET @sql += '
		OFFSET (@PageIndex - 1) * @PageSize ROWS	-- skip  rows
		FETCH NEXT @PageSize ROWS ONLY;				-- take 10 rows
	
		DROP TABLE #searchMovieQuery'
	
	EXECUTE sp_executesql @sql, 
		N'@SearchValue NVARCHAR(200), 
		@PageSize INT, 
		@PageIndex INT, 
		@GenreId INT,
		@CountryId INT,
		@MovieTypeId INT,
		@MovieMainCategoryId INT,
		@TotalCount INT OUTPUT, 
		@TotalPages INT OUTPUT', 
		@SearchValue, 
		@PageSize, 
		@PageIndex,
		@GenreId,
		@CountryId,
		@MovieTypeId,
		@MovieMainCategoryId,
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