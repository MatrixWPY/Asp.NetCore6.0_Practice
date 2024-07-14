USE [WebApi]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matrix
-- Create date: 2023/03/28
-- Description:	Remove ContactInfo
-- =============================================
CREATE PROCEDURE [dbo].[Sp_RemoveContactInfo]
	@ContactInfoIDs	VARCHAR(MAX)
AS
BEGIN
    DELETE FROM
		dbo.Tbl_ContactInfo
	WHERE
		ContactInfoID IN (SELECT CAST(VALUE AS BIGINT) FROM STRING_SPLIT(@ContactInfoIDs, ','))
END
GO
