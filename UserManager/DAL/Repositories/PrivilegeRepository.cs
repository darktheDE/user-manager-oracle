using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý Grant/Revoke Privileges
/// </summary>
public class PrivilegeRepository : BaseRepository
{
    #region System Privileges

    /// <summary>
    /// Lấy tất cả System Privileges đã được gán
    /// </summary>
    public DataTable GetAllSystemPrivileges()
    {
        const string sql = @"
            SELECT 
                GRANTEE,
                PRIVILEGE,
                ADMIN_OPTION,
                CASE 
                    WHEN EXISTS (SELECT 1 FROM DBA_USERS u WHERE u.USERNAME = sp.GRANTEE) THEN 'USER'
                    WHEN EXISTS (SELECT 1 FROM DBA_ROLES r WHERE r.ROLE = sp.GRANTEE) THEN 'ROLE'
                    ELSE 'OTHER'
                END AS GRANTEE_TYPE
            FROM DBA_SYS_PRIVS sp
            ORDER BY GRANTEE, PRIVILEGE";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy System Privileges của một User/Role
    /// </summary>
    public DataTable GetSystemPrivileges(string grantee)
    {
        const string sql = @"
            SELECT 
                PRIVILEGE,
                ADMIN_OPTION
            FROM DBA_SYS_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:grantee)
            ORDER BY PRIVILEGE";

        return ExecuteQuery(sql, new OracleParameter("grantee", grantee));
    }

    /// <summary>
    /// Grant System Privilege (gọi Stored Procedure)
    /// </summary>
    public void GrantSystemPrivilege(string privilege, string grantee, bool withAdminOption = false)
    {
        ExecuteProcedure("SYSTEM.SP_GRANT_SYS_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_grantee", grantee),
            new OracleParameter("p_admin_option", withAdminOption ? 1 : 0)
        );
    }

    /// <summary>
    /// Revoke System Privilege (gọi Stored Procedure)
    /// </summary>
    public void RevokeSystemPrivilege(string privilege, string grantee)
    {
        ExecuteProcedure("SYSTEM.SP_REVOKE_SYS_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_grantee", grantee)
        );
    }

    #endregion

    #region Object Privileges

    /// <summary>
    /// Lấy tất cả Object Privileges đã được gán
    /// </summary>
    public DataTable GetAllObjectPrivileges()
    {
        const string sql = @"
            SELECT 
                GRANTEE,
                OWNER,
                TABLE_NAME,
                PRIVILEGE,
                GRANTABLE,
                GRANTOR,
                CASE 
                    WHEN EXISTS (SELECT 1 FROM DBA_USERS u WHERE u.USERNAME = tp.GRANTEE) THEN 'USER'
                    WHEN EXISTS (SELECT 1 FROM DBA_ROLES r WHERE r.ROLE = tp.GRANTEE) THEN 'ROLE'
                    ELSE 'OTHER'
                END AS GRANTEE_TYPE
            FROM DBA_TAB_PRIVS tp
            ORDER BY GRANTEE, OWNER, TABLE_NAME, PRIVILEGE";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy Object Privileges của một User/Role
    /// </summary>
    public DataTable GetObjectPrivileges(string grantee)
    {
        const string sql = @"
            SELECT 
                OWNER,
                TABLE_NAME,
                PRIVILEGE,
                GRANTABLE,
                GRANTOR
            FROM DBA_TAB_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:grantee)
            ORDER BY OWNER, TABLE_NAME, PRIVILEGE";

        return ExecuteQuery(sql, new OracleParameter("grantee", grantee));
    }

    /// <summary>
    /// Grant Object Privilege trên Table (gọi Stored Procedure)
    /// </summary>
    public void GrantObjectPrivilege(string privilege, string owner, string objectName, string grantee, bool withGrantOption = false)
    {
        ExecuteProcedure("SYSTEM.SP_GRANT_OBJ_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_owner", owner),
            new OracleParameter("p_object_name", objectName),
            new OracleParameter("p_grantee", grantee),
            new OracleParameter("p_grant_option", withGrantOption ? 1 : 0)
        );
    }

    /// <summary>
    /// Revoke Object Privilege trên Table (gọi Stored Procedure)
    /// </summary>
    public void RevokeObjectPrivilege(string privilege, string owner, string objectName, string grantee)
    {
        ExecuteProcedure("SYSTEM.SP_REVOKE_OBJ_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_owner", owner),
            new OracleParameter("p_object_name", objectName),
            new OracleParameter("p_grantee", grantee)
        );
    }

    #endregion

    #region Column Privileges

    /// <summary>
    /// Lấy Column Privileges
    /// </summary>
    public DataTable GetColumnPrivileges(string grantee)
    {
        const string sql = @"
            SELECT 
                TABLE_SCHEMA AS OWNER,
                TABLE_NAME,
                COLUMN_NAME,
                PRIVILEGE,
                GRANTABLE
            FROM DBA_COL_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:grantee)
            ORDER BY TABLE_NAME, COLUMN_NAME";

        return ExecuteQuery(sql, new OracleParameter("grantee", grantee));
    }

    /// <summary>
    /// Grant Column Privilege (gọi Stored Procedure)
    /// </summary>
    public void GrantColumnPrivilege(string privilege, string owner, string tableName, string columnName, string grantee)
    {
        ExecuteProcedure("SYSTEM.SP_GRANT_COL_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_owner", owner),
            new OracleParameter("p_object_name", tableName),
            new OracleParameter("p_column_name", columnName),
            new OracleParameter("p_grantee", grantee),
            new OracleParameter("p_grant_option", 0)
        );
    }

    /// <summary>
    /// Revoke Column Privilege - Phải revoke toàn bộ object privilege
    /// </summary>
    public void RevokeColumnPrivilege(string privilege, string owner, string tableName, string columnName, string grantee)
    {
        // Oracle không hỗ trợ revoke column privilege riêng lẻ
        // Phải revoke toàn bộ privilege trên object
        ExecuteProcedure("SYSTEM.SP_REVOKE_OBJ_PRIV",
            new OracleParameter("p_privilege", privilege),
            new OracleParameter("p_owner", owner),
            new OracleParameter("p_object_name", tableName),
            new OracleParameter("p_grantee", grantee)
        );
    }

    #endregion

    #region Role Grants

    /// <summary>
    /// Lấy danh sách Roles được gán cho User/Role
    /// </summary>
    public DataTable GetGrantedRoles(string grantee)
    {
        const string sql = @"
            SELECT 
                GRANTED_ROLE,
                ADMIN_OPTION,
                DEFAULT_ROLE
            FROM DBA_ROLE_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:grantee)
            ORDER BY GRANTED_ROLE";

        return ExecuteQuery(sql, new OracleParameter("grantee", grantee));
    }

    /// <summary>
    /// Grant Role cho User/Role (gọi Stored Procedure)
    /// </summary>
    public void GrantRole(string roleName, string grantee, bool withAdminOption = false)
    {
        ExecuteProcedure("SYSTEM.SP_GRANT_ROLE",
            new OracleParameter("p_role_name", roleName),
            new OracleParameter("p_grantee", grantee),
            new OracleParameter("p_admin_option", withAdminOption ? 1 : 0)
        );
    }

    /// <summary>
    /// Revoke Role từ User/Role (gọi Stored Procedure)
    /// </summary>
    public void RevokeRole(string roleName, string grantee)
    {
        ExecuteProcedure("SYSTEM.SP_REVOKE_ROLE",
            new OracleParameter("p_role_name", roleName),
            new OracleParameter("p_grantee", grantee)
        );
    }

    #endregion

    #region Check Privileges

    /// <summary>
    /// Kiểm tra User có System Privilege không (trực tiếp hoặc qua Role)
    /// </summary>
    public bool HasSystemPrivilege(string username, string privilege)
    {
        // Kiểm tra quyền trực tiếp
        const string sqlDirect = @"
            SELECT COUNT(*) FROM DBA_SYS_PRIVS 
            WHERE UPPER(GRANTEE) = UPPER(:username) 
              AND UPPER(PRIVILEGE) = UPPER(:privilege)";

        var directCount = Convert.ToInt32(ExecuteScalar(sqlDirect, 
            new OracleParameter("username", username),
            new OracleParameter("privilege", privilege)));

        if (directCount > 0) return true;

        // Kiểm tra quyền qua Role
        const string sqlViaRole = @"
            SELECT COUNT(*) 
            FROM DBA_ROLE_PRIVS rp
            JOIN DBA_SYS_PRIVS sp ON UPPER(sp.GRANTEE) = UPPER(rp.GRANTED_ROLE)
            WHERE UPPER(rp.GRANTEE) = UPPER(:username)
              AND UPPER(sp.PRIVILEGE) = UPPER(:privilege)";

        var roleCount = Convert.ToInt32(ExecuteScalar(sqlViaRole,
            new OracleParameter("username", username),
            new OracleParameter("privilege", privilege)));

        return roleCount > 0;
    }

    /// <summary>
    /// Lấy tất cả quyền của User (cả trực tiếp và qua Role)
    /// </summary>
    public DataTable GetAllUserPrivileges(string username)
    {
        const string sql = @"
            -- System Privileges trực tiếp
            SELECT 
                PRIVILEGE AS PRIVILEGE_NAME,
                'SYSTEM' AS PRIVILEGE_TYPE,
                'DIRECT' AS SOURCE,
                NULL AS SOURCE_ROLE,
                ADMIN_OPTION
            FROM DBA_SYS_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:username1)
            
            UNION ALL
            
            -- System Privileges qua Role
            SELECT 
                sp.PRIVILEGE AS PRIVILEGE_NAME,
                'SYSTEM' AS PRIVILEGE_TYPE,
                'ROLE' AS SOURCE,
                rp.GRANTED_ROLE AS SOURCE_ROLE,
                sp.ADMIN_OPTION
            FROM DBA_ROLE_PRIVS rp
            JOIN DBA_SYS_PRIVS sp ON UPPER(sp.GRANTEE) = UPPER(rp.GRANTED_ROLE)
            WHERE UPPER(rp.GRANTEE) = UPPER(:username2)
            
            UNION ALL
            
            -- Object Privileges trực tiếp
            SELECT 
                PRIVILEGE || ' ON ' || OWNER || '.' || TABLE_NAME AS PRIVILEGE_NAME,
                'OBJECT' AS PRIVILEGE_TYPE,
                'DIRECT' AS SOURCE,
                NULL AS SOURCE_ROLE,
                GRANTABLE AS ADMIN_OPTION
            FROM DBA_TAB_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:username3)
            
            ORDER BY PRIVILEGE_TYPE, SOURCE, PRIVILEGE_NAME";

        return ExecuteQuery(sql,
            new OracleParameter("username1", username),
            new OracleParameter("username2", username),
            new OracleParameter("username3", username));
    }

    #endregion
}
