-- ============================================
-- STORED PROCEDURES CHO ỨNG DỤNG QUẢN LÝ USER
-- Tất cả procedures sử dụng AUTHID CURRENT_USER
-- để chạy với quyền của người gọi
-- ============================================

-- ============================================
-- 1. PROCEDURES CHO USER MANAGEMENT
-- ============================================

-- Tạo User mới
CREATE OR REPLACE PROCEDURE SP_CREATE_USER (
    p_username          IN VARCHAR2,
    p_password          IN VARCHAR2,
    p_default_ts        IN VARCHAR2 DEFAULT 'USERS',
    p_temp_ts           IN VARCHAR2 DEFAULT 'TEMP',
    p_profile           IN VARCHAR2 DEFAULT 'DEFAULT',
    p_quota             IN VARCHAR2 DEFAULT 'UNLIMITED',
    p_account_lock      IN NUMBER DEFAULT 0
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(4000);
BEGIN
    v_sql := 'CREATE USER "' || p_username || '" IDENTIFIED BY "' || p_password || '"';
    v_sql := v_sql || ' DEFAULT TABLESPACE "' || p_default_ts || '"';
    v_sql := v_sql || ' TEMPORARY TABLESPACE "' || p_temp_ts || '"';
    v_sql := v_sql || ' PROFILE "' || p_profile || '"';
    
    IF UPPER(p_quota) = 'UNLIMITED' THEN
        v_sql := v_sql || ' QUOTA UNLIMITED ON "' || p_default_ts || '"';
    ELSE
        v_sql := v_sql || ' QUOTA ' || p_quota || ' ON "' || p_default_ts || '"';
    END IF;
    
    IF p_account_lock = 1 THEN
        v_sql := v_sql || ' ACCOUNT LOCK';
    END IF;
    
    EXECUTE IMMEDIATE v_sql;
END SP_CREATE_USER;
/

-- Cập nhật User
CREATE OR REPLACE PROCEDURE SP_UPDATE_USER (
    p_username          IN VARCHAR2,
    p_password          IN VARCHAR2 DEFAULT NULL,
    p_default_ts        IN VARCHAR2 DEFAULT NULL,
    p_temp_ts           IN VARCHAR2 DEFAULT NULL,
    p_profile           IN VARCHAR2 DEFAULT NULL,
    p_quota             IN VARCHAR2 DEFAULT NULL
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(1000);
    v_default_ts VARCHAR2(128);
BEGIN
    IF p_password IS NOT NULL THEN
        v_sql := 'ALTER USER "' || p_username || '" IDENTIFIED BY "' || p_password || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;
    
    IF p_default_ts IS NOT NULL THEN
        v_sql := 'ALTER USER "' || p_username || '" DEFAULT TABLESPACE "' || p_default_ts || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;
    
    IF p_temp_ts IS NOT NULL THEN
        v_sql := 'ALTER USER "' || p_username || '" TEMPORARY TABLESPACE "' || p_temp_ts || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;
    
    IF p_profile IS NOT NULL THEN
        v_sql := 'ALTER USER "' || p_username || '" PROFILE "' || p_profile || '"';
        EXECUTE IMMEDIATE v_sql;
    END IF;
    
    -- Xử lý Quota: nếu có p_quota, lấy default_ts từ param hoặc từ database
    IF p_quota IS NOT NULL THEN
        -- Lấy default tablespace: ưu tiên param, nếu không có thì lấy từ DB
        IF p_default_ts IS NOT NULL THEN
            v_default_ts := p_default_ts;
        ELSE
            SELECT DEFAULT_TABLESPACE INTO v_default_ts 
            FROM DBA_USERS 
            WHERE UPPER(USERNAME) = UPPER(p_username);
        END IF;
        
        IF UPPER(p_quota) = 'UNLIMITED' THEN
            v_sql := 'ALTER USER "' || p_username || '" QUOTA UNLIMITED ON "' || v_default_ts || '"';
        ELSE
            v_sql := 'ALTER USER "' || p_username || '" QUOTA ' || p_quota || ' ON "' || v_default_ts || '"';
        END IF;
        EXECUTE IMMEDIATE v_sql;
    END IF;
END SP_UPDATE_USER;
/

-- Xóa User (kill sessions trước khi drop)
CREATE OR REPLACE PROCEDURE SP_DELETE_USER (
    p_username IN VARCHAR2
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    -- Kill tất cả sessions của user trước
    FOR sess IN (
        SELECT SID, SERIAL#
        FROM V$SESSION
        WHERE UPPER(USERNAME) = UPPER(p_username)
    ) LOOP
        BEGIN
            v_sql := 'ALTER SYSTEM KILL SESSION ''' || sess.SID || ',' || sess.SERIAL# || ''' IMMEDIATE';
            EXECUTE IMMEDIATE v_sql;
        EXCEPTION
            WHEN OTHERS THEN
                -- Bỏ qua lỗi nếu session đã kết thúc
                NULL;
        END;
    END LOOP;
    
    -- Drop user
    EXECUTE IMMEDIATE 'DROP USER "' || p_username || '" CASCADE';
END SP_DELETE_USER;
/

-- Lock User
CREATE OR REPLACE PROCEDURE SP_LOCK_USER (
    p_username IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER USER "' || p_username || '" ACCOUNT LOCK';
END SP_LOCK_USER;
/

-- Unlock User
CREATE OR REPLACE PROCEDURE SP_UNLOCK_USER (
    p_username IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER USER "' || p_username || '" ACCOUNT UNLOCK';
END SP_UNLOCK_USER;
/

-- Đổi Password User
CREATE OR REPLACE PROCEDURE SP_CHANGE_PASSWORD (
    p_username      IN VARCHAR2,
    p_new_password  IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER USER "' || p_username || '" IDENTIFIED BY "' || p_new_password || '"';
END SP_CHANGE_PASSWORD;
/

-- ============================================
-- 2. PROCEDURES CHO ROLE MANAGEMENT
-- ============================================

-- Tạo Role (không password)
CREATE OR REPLACE PROCEDURE SP_CREATE_ROLE (
    p_role_name IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE "' || p_role_name || '"';
END SP_CREATE_ROLE;
/

-- Tạo Role với password
CREATE OR REPLACE PROCEDURE SP_CREATE_ROLE_WITH_PASSWORD (
    p_role_name IN VARCHAR2,
    p_password  IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE "' || p_role_name || '" IDENTIFIED BY "' || p_password || '"';
END SP_CREATE_ROLE_WITH_PASSWORD;
/

-- Đổi password Role
CREATE OR REPLACE PROCEDURE SP_CHANGE_ROLE_PASSWORD (
    p_role_name     IN VARCHAR2,
    p_new_password  IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER ROLE "' || p_role_name || '" IDENTIFIED BY "' || p_new_password || '"';
END SP_CHANGE_ROLE_PASSWORD;
/

-- Xóa password Role
CREATE OR REPLACE PROCEDURE SP_REMOVE_ROLE_PASSWORD (
    p_role_name IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER ROLE "' || p_role_name || '" NOT IDENTIFIED';
END SP_REMOVE_ROLE_PASSWORD;
/

-- Xóa Role
CREATE OR REPLACE PROCEDURE SP_DELETE_ROLE (
    p_role_name IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'DROP ROLE "' || p_role_name || '"';
END SP_DELETE_ROLE;
/

-- ============================================
-- 3. PROCEDURES CHO PROFILE MANAGEMENT
-- ============================================

-- Tạo Profile
CREATE OR REPLACE PROCEDURE SP_CREATE_PROFILE (
    p_profile_name      IN VARCHAR2,
    p_sessions_per_user IN VARCHAR2 DEFAULT 'UNLIMITED',
    p_connect_time      IN VARCHAR2 DEFAULT 'UNLIMITED',
    p_idle_time         IN VARCHAR2 DEFAULT 'UNLIMITED'
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(1000);
BEGIN
    v_sql := 'CREATE PROFILE "' || p_profile_name || '" LIMIT';
    v_sql := v_sql || ' SESSIONS_PER_USER ' || p_sessions_per_user;
    v_sql := v_sql || ' CONNECT_TIME ' || p_connect_time;
    v_sql := v_sql || ' IDLE_TIME ' || p_idle_time;
    
    EXECUTE IMMEDIATE v_sql;
END SP_CREATE_PROFILE;
/

-- Cập nhật Profile
CREATE OR REPLACE PROCEDURE SP_UPDATE_PROFILE (
    p_profile_name      IN VARCHAR2,
    p_sessions_per_user IN VARCHAR2 DEFAULT NULL,
    p_connect_time      IN VARCHAR2 DEFAULT NULL,
    p_idle_time         IN VARCHAR2 DEFAULT NULL
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(1000);
BEGIN
    v_sql := 'ALTER PROFILE "' || p_profile_name || '" LIMIT';
    
    IF p_sessions_per_user IS NOT NULL THEN
        v_sql := v_sql || ' SESSIONS_PER_USER ' || p_sessions_per_user;
    END IF;
    
    IF p_connect_time IS NOT NULL THEN
        v_sql := v_sql || ' CONNECT_TIME ' || p_connect_time;
    END IF;
    
    IF p_idle_time IS NOT NULL THEN
        v_sql := v_sql || ' IDLE_TIME ' || p_idle_time;
    END IF;
    
    EXECUTE IMMEDIATE v_sql;
END SP_UPDATE_PROFILE;
/

-- Xóa Profile
CREATE OR REPLACE PROCEDURE SP_DELETE_PROFILE (
    p_profile_name IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'DROP PROFILE "' || p_profile_name || '" CASCADE';
END SP_DELETE_PROFILE;
/

-- ============================================
-- 4. PROCEDURES CHO PRIVILEGE MANAGEMENT
-- ============================================

-- Grant System Privilege
CREATE OR REPLACE PROCEDURE SP_GRANT_SYS_PRIV (
    p_privilege     IN VARCHAR2,
    p_grantee       IN VARCHAR2,
    p_admin_option  IN NUMBER DEFAULT 0
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT ' || p_privilege || ' TO "' || p_grantee || '"';
    IF p_admin_option = 1 THEN
        v_sql := v_sql || ' WITH ADMIN OPTION';
    END IF;
    EXECUTE IMMEDIATE v_sql;
END SP_GRANT_SYS_PRIV;
/

-- Revoke System Privilege
CREATE OR REPLACE PROCEDURE SP_REVOKE_SYS_PRIV (
    p_privilege IN VARCHAR2,
    p_grantee   IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'REVOKE ' || p_privilege || ' FROM "' || p_grantee || '"';
END SP_REVOKE_SYS_PRIV;
/

-- Grant Object Privilege
CREATE OR REPLACE PROCEDURE SP_GRANT_OBJ_PRIV (
    p_privilege     IN VARCHAR2,
    p_owner         IN VARCHAR2,
    p_object_name   IN VARCHAR2,
    p_grantee       IN VARCHAR2,
    p_grant_option  IN NUMBER DEFAULT 0
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT ' || p_privilege || ' ON "' || p_owner || '"."' || p_object_name || '" TO "' || p_grantee || '"';
    IF p_grant_option = 1 THEN
        v_sql := v_sql || ' WITH GRANT OPTION';
    END IF;
    EXECUTE IMMEDIATE v_sql;
END SP_GRANT_OBJ_PRIV;
/

-- Revoke Object Privilege
CREATE OR REPLACE PROCEDURE SP_REVOKE_OBJ_PRIV (
    p_privilege     IN VARCHAR2,
    p_owner         IN VARCHAR2,
    p_object_name   IN VARCHAR2,
    p_grantee       IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'REVOKE ' || p_privilege || ' ON "' || p_owner || '"."' || p_object_name || '" FROM "' || p_grantee || '"';
END SP_REVOKE_OBJ_PRIV;
/

-- Grant Column Privilege
CREATE OR REPLACE PROCEDURE SP_GRANT_COL_PRIV (
    p_privilege     IN VARCHAR2,
    p_owner         IN VARCHAR2,
    p_object_name   IN VARCHAR2,
    p_column_name   IN VARCHAR2,
    p_grantee       IN VARCHAR2,
    p_grant_option  IN NUMBER DEFAULT 0
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT ' || p_privilege || '("' || p_column_name || '") ON "' || p_owner || '"."' || p_object_name || '" TO "' || p_grantee || '"';
    IF p_grant_option = 1 THEN
        v_sql := v_sql || ' WITH GRANT OPTION';
    END IF;
    EXECUTE IMMEDIATE v_sql;
END SP_GRANT_COL_PRIV;
/

-- Grant Role to User/Role
CREATE OR REPLACE PROCEDURE SP_GRANT_ROLE (
    p_role_name     IN VARCHAR2,
    p_grantee       IN VARCHAR2,
    p_admin_option  IN NUMBER DEFAULT 0
)
AUTHID CURRENT_USER
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT "' || p_role_name || '" TO "' || p_grantee || '"';
    IF p_admin_option = 1 THEN
        v_sql := v_sql || ' WITH ADMIN OPTION';
    END IF;
    EXECUTE IMMEDIATE v_sql;
END SP_GRANT_ROLE;
/

-- Revoke Role from User/Role
CREATE OR REPLACE PROCEDURE SP_REVOKE_ROLE (
    p_role_name IN VARCHAR2,
    p_grantee   IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    EXECUTE IMMEDIATE 'REVOKE "' || p_role_name || '" FROM "' || p_grantee || '"';
END SP_REVOKE_ROLE;
/

-- ============================================
-- 5. PROCEDURES CHO USER_INFO (Thông tin cá nhân)
-- ============================================

-- Thêm User Info
CREATE OR REPLACE PROCEDURE SP_INSERT_USER_INFO (
    p_username      IN VARCHAR2,
    p_ho_ten        IN NVARCHAR2,
    p_ngay_sinh     IN DATE DEFAULT NULL,
    p_gioi_tinh     IN VARCHAR2 DEFAULT NULL,
    p_dia_chi       IN NVARCHAR2 DEFAULT NULL,
    p_so_dien_thoai IN VARCHAR2 DEFAULT NULL,
    p_email         IN VARCHAR2 DEFAULT NULL,
    p_chuc_vu       IN NVARCHAR2 DEFAULT NULL,
    p_phong_ban     IN NVARCHAR2 DEFAULT NULL,
    p_ma_nhan_vien  IN VARCHAR2 DEFAULT NULL,
    p_ghi_chu       IN NVARCHAR2 DEFAULT NULL,
    p_created_by    IN VARCHAR2 DEFAULT 'SYSTEM'
)
AUTHID CURRENT_USER
AS
BEGIN
    INSERT INTO SYSTEM.USER_INFO (
        USER_INFO_ID, USERNAME, HO_TEN, NGAY_SINH, GIOI_TINH,
        DIA_CHI, SO_DIEN_THOAI, EMAIL, CHUC_VU, PHONG_BAN,
        MA_NHAN_VIEN, GHI_CHU, CREATED_BY
    ) VALUES (
        SYSTEM.SEQ_USER_INFO.NEXTVAL, UPPER(p_username), p_ho_ten, p_ngay_sinh, p_gioi_tinh,
        p_dia_chi, p_so_dien_thoai, p_email, p_chuc_vu, p_phong_ban,
        p_ma_nhan_vien, p_ghi_chu, p_created_by
    );
    COMMIT;
END SP_INSERT_USER_INFO;
/

-- Cập nhật User Info
CREATE OR REPLACE PROCEDURE SP_UPDATE_USER_INFO (
    p_username      IN VARCHAR2,
    p_ho_ten        IN NVARCHAR2,
    p_ngay_sinh     IN DATE DEFAULT NULL,
    p_gioi_tinh     IN VARCHAR2 DEFAULT NULL,
    p_dia_chi       IN NVARCHAR2 DEFAULT NULL,
    p_so_dien_thoai IN VARCHAR2 DEFAULT NULL,
    p_email         IN VARCHAR2 DEFAULT NULL,
    p_chuc_vu       IN NVARCHAR2 DEFAULT NULL,
    p_phong_ban     IN NVARCHAR2 DEFAULT NULL,
    p_ma_nhan_vien  IN VARCHAR2 DEFAULT NULL,
    p_ghi_chu       IN NVARCHAR2 DEFAULT NULL,
    p_updated_by    IN VARCHAR2 DEFAULT 'SYSTEM'
)
AUTHID CURRENT_USER
AS
BEGIN
    UPDATE SYSTEM.USER_INFO SET
        HO_TEN = p_ho_ten,
        NGAY_SINH = p_ngay_sinh,
        GIOI_TINH = p_gioi_tinh,
        DIA_CHI = p_dia_chi,
        SO_DIEN_THOAI = p_so_dien_thoai,
        EMAIL = p_email,
        CHUC_VU = p_chuc_vu,
        PHONG_BAN = p_phong_ban,
        MA_NHAN_VIEN = p_ma_nhan_vien,
        GHI_CHU = p_ghi_chu,
        UPDATED_BY = p_updated_by,
        UPDATED_DATE = SYSDATE
    WHERE UPPER(USERNAME) = UPPER(p_username);
    COMMIT;
END SP_UPDATE_USER_INFO;
/

CREATE OR REPLACE PROCEDURE SP_DELETE_USER_INFO (
    p_username      IN VARCHAR2,
    p_updated_by    IN VARCHAR2 DEFAULT 'SYSTEM'
)
AUTHID CURRENT_USER
AS
BEGIN
    UPDATE SYSTEM.USER_INFO SET 
        IS_ACTIVE = 0,
        UPDATED_BY = p_updated_by,
        UPDATED_DATE = SYSDATE
    WHERE UPPER(USERNAME) = UPPER(p_username);
    COMMIT;
END SP_DELETE_USER_INFO;
/

-- Xóa User Info (hard delete)
CREATE OR REPLACE PROCEDURE SP_HARD_DELETE_USER_INFO (
    p_username IN VARCHAR2
)
AUTHID CURRENT_USER
AS
BEGIN
    DELETE FROM SYSTEM.USER_INFO WHERE UPPER(USERNAME) = UPPER(p_username);
    COMMIT;
END SP_HARD_DELETE_USER_INFO;
/

COMMIT;

-- ============================================
-- GRANT QUYỀN EXECUTE CHO CÁC STORED PROCEDURES
-- ============================================

-- Grant execute cho tất cả users (thông qua PUBLIC)
GRANT EXECUTE ON SP_CREATE_USER TO PUBLIC;
GRANT EXECUTE ON SP_UPDATE_USER TO PUBLIC;
GRANT EXECUTE ON SP_DELETE_USER TO PUBLIC;
GRANT EXECUTE ON SP_LOCK_USER TO PUBLIC;
GRANT EXECUTE ON SP_UNLOCK_USER TO PUBLIC;
GRANT EXECUTE ON SP_CHANGE_PASSWORD TO PUBLIC;

GRANT EXECUTE ON SP_CREATE_ROLE TO PUBLIC;
GRANT EXECUTE ON SP_CREATE_ROLE_WITH_PASSWORD TO PUBLIC;
GRANT EXECUTE ON SP_CHANGE_ROLE_PASSWORD TO PUBLIC;
GRANT EXECUTE ON SP_REMOVE_ROLE_PASSWORD TO PUBLIC;
GRANT EXECUTE ON SP_DELETE_ROLE TO PUBLIC;

GRANT EXECUTE ON SP_CREATE_PROFILE TO PUBLIC;
GRANT EXECUTE ON SP_UPDATE_PROFILE TO PUBLIC;
GRANT EXECUTE ON SP_DELETE_PROFILE TO PUBLIC;

GRANT EXECUTE ON SP_GRANT_SYS_PRIV TO PUBLIC;
GRANT EXECUTE ON SP_REVOKE_SYS_PRIV TO PUBLIC;
GRANT EXECUTE ON SP_GRANT_OBJ_PRIV TO PUBLIC;
GRANT EXECUTE ON SP_REVOKE_OBJ_PRIV TO PUBLIC;
GRANT EXECUTE ON SP_GRANT_COL_PRIV TO PUBLIC;
GRANT EXECUTE ON SP_GRANT_ROLE TO PUBLIC;
GRANT EXECUTE ON SP_REVOKE_ROLE TO PUBLIC;

GRANT EXECUTE ON SP_INSERT_USER_INFO TO PUBLIC;
GRANT EXECUTE ON SP_UPDATE_USER_INFO TO PUBLIC;
GRANT EXECUTE ON SP_DELETE_USER_INFO TO PUBLIC;
GRANT EXECUTE ON SP_HARD_DELETE_USER_INFO TO PUBLIC;

-- Grant SELECT trên USER_INFO cho tất cả users
GRANT SELECT ON SYSTEM.USER_INFO TO PUBLIC;
GRANT SELECT ON SYSTEM.SEQ_USER_INFO TO PUBLIC;

COMMIT;

-- Kiểm tra các procedures đã tạo
SELECT object_name, object_type, status 
FROM user_objects 
WHERE object_type = 'PROCEDURE' 
AND object_name LIKE 'SP_%'
ORDER BY object_name;

