USE `WebApi`;
DROP procedure IF EXISTS `Sp_EditContactInfo`;

DELIMITER $$

USE `WebApi`$$
CREATE PROCEDURE `Sp_EditContactInfo`
(
	IN In_ContactInfoID BIGINT,
	IN In_Name			VARCHAR(10),
	IN In_Nickname		VARCHAR(10),
	IN In_Gender		TINYINT,
	IN In_Age			TINYINT,
	IN In_PhoneNo		VARCHAR(20),
    IN In_Address		VARCHAR(100)
)
BEGIN
	UPDATE Tbl_ContactInfo SET
		Name = In_Name,
		Nickname = In_Nickname,
		Gender = In_Gender,
		Age = In_Age,
		PhoneNo = In_PhoneNo,
		Address = In_Address,
		UpdateTime = NOW()
	WHERE
		ContactInfoID = In_ContactInfoID;
END$$

DELIMITER ;

