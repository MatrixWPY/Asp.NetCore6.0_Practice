CREATE OR REPLACE PROCEDURE SP_INSERT_CONTACTINFO
(
    i_name VARCHAR2,
    i_nickname VARCHAR2 DEFAULT NULL,
    i_gender NUMBER DEFAULT NULL,
    i_age NUMBER DEFAULT NULL,
    i_phoneNo VARCHAR2,
    i_address NVARCHAR2,
    o_contactInfoID OUT NUMBER
)
AS 
BEGIN

    INSERT INTO TBL_CONTACTINFO
        (NAME, NICKNAME, GENDER, AGE, PHONE_NO, ADDRESS)
    VALUES
        (i_name, i_nickname, i_gender, i_age, i_phoneNo, i_address)
    RETURNING CONTACTINFO_ID INTO o_contactInfoID;

END SP_INSERT_CONTACTINFO;