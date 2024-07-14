USE `WebApi`;
DROP procedure IF EXISTS `Sp_ListContactInfo`;

DELIMITER $$

USE `WebApi`$$
CREATE PROCEDURE `Sp_ListContactInfo`
(
	IN In_Name		VARCHAR(10),
	IN In_Nickname	VARCHAR(10),
	IN In_Gender	TINYINT,
	IN In_RowStart	INT,
	IN In_RowLength	INT
)
BEGIN
	CREATE TEMPORARY TABLE IF NOT EXISTS TmpTable AS
    (
		SELECT
			*
		FROM
			Tbl_ContactInfo
		WHERE
			1 = 1
		AND
			(
				(In_Name IS NULL OR In_Name = '')
				OR
				(Name = In_Name)
			)
		AND
			(
				(In_Nickname IS NULL OR In_Nickname = '')
				OR
				(Nickname LIKE CONCAT('%', In_Nickname, '%'))
			)
		AND
			(
				(In_Gender IS NULL)
				OR
				(Gender = In_Gender)
			)
	);

	SELECT COUNT(1) FROM TmpTable;
	SELECT * FROM TmpTable ORDER BY ContactInfoID DESC LIMIT In_RowStart, In_RowLength;

	DROP TABLE IF EXISTS TmpTable;
END$$

DELIMITER ;

