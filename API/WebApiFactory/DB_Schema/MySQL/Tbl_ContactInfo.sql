CREATE TABLE `WebApi`.`Tbl_ContactInfo` (
  `ContactInfoID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(10) NOT NULL,
  `Nickname` varchar(10) NULL,
  `Gender` tinyint(1) NULL,
  `Age` tinyint NULL,
  `PhoneNo` varchar(20) NOT NULL,
  `Address` varchar(100) NOT NULL,
  `IsEnable` tinyint(1) NOT NULL DEFAULT 1,
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`ContactInfoID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci