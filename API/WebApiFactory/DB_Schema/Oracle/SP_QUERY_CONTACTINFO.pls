CREATE OR REPLACE PROCEDURE SP_QUERY_CONTACTINFO
(
    i_name VARCHAR2 DEFAULT NULL,
    i_nickname VARCHAR2 DEFAULT NULL,
    i_gender NUMBER DEFAULT NULL,
    i_rowStart NUMBER DEFAULT 0,
    i_rowLength NUMBER DEFAULT 10,
    o_cnt OUT SYS_REFCURSOR,
    o_result OUT SYS_REFCURSOR
)
AS 
BEGIN

    OPEN o_cnt FOR
    SELECT
        COUNT(1)
    FROM
        TBL_CONTACTINFO
    WHERE 1 = 1
    AND
    (
        (i_name IS NULL)
        OR
        (NAME = i_name)
    )
    AND
    (
        (i_nickname IS NULL)
        OR
        (NICKNAME LIKE i_nickname)
    )
    AND
    (
        (i_gender IS NULL)
        OR
        (GENDER = i_gender)
    );

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
    WHERE 1 = 1
    AND
    (
        (i_name IS NULL)
        OR
        (NAME = i_name)
    )
    AND
    (
        (i_nickname IS NULL)
        OR
        (NICKNAME LIKE i_nickname)
    )
    AND
    (
        (i_gender IS NULL)
        OR
        (GENDER = i_gender)
    )
    ORDER BY
        ContactInfoID DESC
    OFFSET i_rowStart ROWS FETCH NEXT i_rowLength ROWS ONLY;

END SP_QUERY_CONTACTINFO;