USE [WebApi]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matrix
-- Create date: 2023/03/28
-- Description:	Edit ContactInfo
-- =============================================
CREATE PROCEDURE [dbo].[Sp_EditContactInfo]
	@ContactInfoID	BIGINT,
	@Name			NVARCHAR(10),
	@Nickname		NVARCHAR(10) = NULL,
	@Gender			TINYINT = NULL,
	@Age			TINYINT = NULL,
	@PhoneNo		VARCHAR(20),
	@Address		NVARCHAR(100)
AS
BEGIN
    UPDATE dbo.Tbl_ContactInfo SET
		Name = @Name,
		Nickname = @Nickname,
		Gender = @Gender,
		Age = @Age,
		PhoneNo = @PhoneNo,
		Address = @Address,
		UpdateTime = GETDATE()
	WHERE
		ContactInfoID = @ContactInfoID
END
GO
