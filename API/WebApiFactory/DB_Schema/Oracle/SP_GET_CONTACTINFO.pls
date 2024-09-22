CREATE OR REPLACE PROCEDURE SP_GET_CONTACTINFO
(
    i_contactInfoID NUMBER,
    o_result OUT SYS_REFCURSOR
)
AS 
BEGIN

    OPEN o_result FOR
    SELECT
        CONTACTINFO_ID AS ContactInfoID,
        NAME,
        NICKNAME,
        GENDER,
        AGE,
        PHONE_NO AS PhoneNo,
        ADDRESS,
        IS_ENABLE AS IsEnable,
        CREATE_TIME AS CreateTime,
        UPDATE_TIME AS UpdateTime
    FROM
        TBL_CONTACTINFO
    WHERE
        CONTACTINFO_ID = i_contactInfoID;

END SP_GET_CONTACTINFO;