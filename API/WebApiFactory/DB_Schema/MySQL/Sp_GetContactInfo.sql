USE `WebApi`;
DROP procedure IF EXISTS `Sp_GetContactInfo`;

DELIMITER $$

USE `WebApi`$$
CREATE PROCEDURE `Sp_GetContactInfo`
(
	IN In_ContactInfoID BIGINT
)
BEGIN
	SELECT
		*
	FROM
		Tbl_ContactInfo
	WHERE
		ContactInfoID = In_ContactInfoID
	ORDER BY
		ContactInfoID DESC;
END$$

DELIMITER ;

