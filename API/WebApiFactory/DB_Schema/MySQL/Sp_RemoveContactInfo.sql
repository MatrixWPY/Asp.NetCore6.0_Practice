USE `WebApi`;
DROP procedure IF EXISTS `Sp_RemoveContactInfo`;

DELIMITER $$

USE `WebApi`$$
CREATE PROCEDURE `Sp_RemoveContactInfo`
(
	IN In_ContactInfoIDs VARCHAR(16383)
)
BEGIN
	DELETE FROM
		Tbl_ContactInfo
	WHERE
		FIND_IN_SET(ContactInfoID, In_ContactInfoIDs);
END$$

DELIMITER ;

