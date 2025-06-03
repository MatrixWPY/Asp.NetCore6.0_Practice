USE [WebApi]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matrix
-- Create date: 2023/03/28
-- Description:	List ContactInfo
-- =============================================
CREATE PROCEDURE [dbo].[Sp_ListContactInfo]
	@Name		NVARCHAR(10) = NULL,
	@Nickname	NVARCHAR(10) = NULL,
	@Gender		TINYINT = NULL,
	@RowStart	INT = 0,
	@RowLength	INT = 10
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT
		*
	INTO
		#TempTable
	FROM
		dbo.Tbl_ContactInfo
	WHERE
		1 = 1
	AND
		(
			(@Name IS NULL OR @Name = '')
			OR
			(Name = @Name)
		)
	AND
		(
			(@Nickname IS NULL OR @Nickname = '')
			OR
			(Nickname LIKE '%' + @Nickname + '%')
		)
	AND
		(
			(@Gender IS NULL)
			OR
			(Gender = @Gender)
		)

	SELECT COUNT(1) FROM #TempTable
	SELECT * FROM #TempTable ORDER BY ContactInfoID DESC OFFSET @RowStart ROWS FETCH NEXT @RowLength ROWS ONLY

	DROP TABLE IF EXISTS #TempTable
END
GO
