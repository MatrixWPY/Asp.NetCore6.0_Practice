CREATE OR REPLACE PROCEDURE SP_UPDATE_CONTACTINFO
(
    i_contactInfoID NUMBER,
    i_name VARCHAR2,
    i_nickname VARCHAR2 DEFAULT NULL,
    i_gender NUMBER DEFAULT NULL,
    i_age NUMBER DEFAULT NULL,
    i_phoneNo VARCHAR2,
    i_address NVARCHAR2,
	o_result OUT NUMBER
)
AS 
BEGIN

    UPDATE TBL_CONTACTINFO SET
        NAME = i_name,
        NICKNAME = i_nickname,
        GENDER = i_gender,
        AGE = i_age,
        PHONE_NO = i_phoneNo,
        ADDRESS = i_address,
        UPDATE_TIME = sysdate
    WHERE
        CONTACTINFO_ID = i_contactInfoID;

	o_result := SQL%ROWCOUNT; --受影響行數

END SP_UPDATE_CONTACTINFO;