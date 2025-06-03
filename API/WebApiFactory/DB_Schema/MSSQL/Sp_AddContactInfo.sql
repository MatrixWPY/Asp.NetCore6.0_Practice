USE [WebApi]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matrix
-- Create date: 2023/03/28
-- Description:	Add ContactInfo
-- =============================================
CREATE PROCEDURE [dbo].[Sp_AddContactInfo]
	@Name		NVARCHAR(10),
	@Nickname	NVARCHAR(10) = NULL,
	@Gender		TINYINT = NULL,
	@Age		TINYINT = NULL,
	@PhoneNo	VARCHAR(20),
	@Address	NVARCHAR(100)
AS
BEGIN
    INSERT INTO dbo.Tbl_ContactInfo
		(Name, Nickname, Gender, Age, PhoneNo, Address)
	VALUES
		(@Name, @Nickname, @Gender, @Age, @PhoneNo, @Address)

	SELECT SCOPE_IDENTITY()
END
GO
