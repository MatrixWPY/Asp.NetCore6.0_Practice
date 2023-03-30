USE `WebApi`;
DROP procedure IF EXISTS `Sp_AddContactInfo`;

DELIMITER $$

USE `WebApi`$$
CREATE PROCEDURE `Sp_AddContactInfo`
(
	IN In_Name		VARCHAR(10),
	IN In_Nickname	VARCHAR(10),
	IN In_Gender	TINYINT,
	IN In_Age		TINYINT,
	IN In_PhoneNo	VARCHAR(20),
    IN In_Address	VARCHAR(100)
)
BEGIN
	INSERT INTO Tbl_ContactInfo
		(Name, Nickname, Gender, Age, PhoneNo, Address)
	VALUES
		(In_Name, In_Nickname, In_Gender, In_Age, In_PhoneNo, In_Address);
	
    SELECT LAST_INSERT_ID();
END$$

DELIMITER ;

