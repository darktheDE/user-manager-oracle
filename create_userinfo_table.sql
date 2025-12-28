-- ============================================
-- SCRIPT TẠO BẢNG USER_INFO 
-- Chạy script này sau khi kết nối Oracle
-- ============================================

-- Kết nối: docker exec -it oracle-23ai sqlplus SYSTEM/YourStrongPassword123@FREEPDB1

-- Tạo Sequence cho USER_INFO
CREATE SEQUENCE SEQ_USER_INFO START WITH 1 INCREMENT BY 1 NOCACHE NOCYCLE;

-- Tạo bảng USER_INFO (Thông tin cá nhân bổ sung)
CREATE TABLE USER_INFO (
    USER_INFO_ID        NUMBER(10)          PRIMARY KEY,
    USERNAME            VARCHAR2(128)       NOT NULL UNIQUE,
    HO_TEN              NVARCHAR2(100)      NOT NULL,
    NGAY_SINH           DATE,
    GIOI_TINH           VARCHAR2(10),
    DIA_CHI             NVARCHAR2(255),
    SO_DIEN_THOAI       VARCHAR2(20),
    EMAIL               VARCHAR2(100),
    CHUC_VU             NVARCHAR2(100),
    PHONG_BAN           NVARCHAR2(100),
    MA_NHAN_VIEN        VARCHAR2(20),
    GHI_CHU             NVARCHAR2(500),
    CREATED_DATE        DATE                DEFAULT SYSDATE,
    CREATED_BY          VARCHAR2(128),
    UPDATED_DATE        DATE,
    UPDATED_BY          VARCHAR2(128),
    IS_ACTIVE           NUMBER(1)           DEFAULT 1
);

-- Hiển thị kết quả
SELECT table_name FROM user_tables WHERE table_name = 'USER_INFO';

COMMIT;
