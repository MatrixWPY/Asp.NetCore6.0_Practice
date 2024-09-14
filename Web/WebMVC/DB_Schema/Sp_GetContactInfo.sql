USE [WebMvc]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matrix
-- Create date: 2023/03/28
-- Description:	Get ContactInfo
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetContactInfo]
	@ContactInfoID BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT
		*
	FROM
		dbo.Tbl_ContactInfo
	WHERE
		ContactInfoID = @ContactInfoID
	ORDER BY
		ContactInfoID DESC
END
GO
