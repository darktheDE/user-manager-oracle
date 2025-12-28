using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý Oracle Roles
/// </summary>
public class RoleRepository : BaseRepository
{
    /// <summary>
    /// Lấy danh sách tất cả Roles (không bao gồm Oracle system roles quan trọng)
    /// </summary>
    public DataTable GetAllRoles()
    {
        const string sql = @"
            SELECT 
                ROLE,
                PASSWORD_REQUIRED,
                AUTHENTICATION_TYPE
            FROM DBA_ROLES
            WHERE ROLE NOT IN (
                'DBA', 'CONNECT', 'RESOURCE', 'PUBLIC', 
                'EXP_FULL_DATABASE', 'IMP_FULL_DATABASE',
                'DATAPUMP_EXP_FULL_DATABASE', 'DATAPUMP_IMP_FULL_DATABASE',
                'EXECUTE_CATALOG_ROLE', 'SELECT_CATALOG_ROLE',
                'DELETE_CATALOG_ROLE', 'SCHEDULER_ADMIN',
                'HS_ADMIN_SELECT_ROLE', 'HS_ADMIN_EXECUTE_ROLE', 'HS_ADMIN_ROLE',
                'GLOBAL_AQ_USER_ROLE', 'OEM_ADVISOR', 'OEM_MONITOR',
                'RECOVERY_CATALOG_OWNER', 'RECOVERY_CATALOG_USER',
                'AQ_ADMINISTRATOR_ROLE', 'AQ_USER_ROLE',
                'ADM_PARALLEL_EXECUTE_TASK', 'GATHER_SYSTEM_STATISTICS',
                'LOGSTDBY_ADMINISTRATOR', 'XDBADMIN', 'XDB_SET_INVOKER',
                'AUTHENTICATEDUSER', 'XDB_WEBSERVICES', 'XDB_WEBSERVICES_WITH_PUBLIC',
                'XDB_WEBSERVICES_OVER_HTTP', 'DBFS_ROLE', 'SODA_APP'
            )
            ORDER BY ROLE";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy thông tin Role theo tên
    /// </summary>
    public DataTable GetRoleByName(string roleName)
    {
        const string sql = @"
            SELECT 
                ROLE,
                PASSWORD_REQUIRED,
                AUTHENTICATION_TYPE
            FROM DBA_ROLES
            WHERE UPPER(ROLE) = UPPER(:roleName)";

        return ExecuteQuery(sql, new OracleParameter("roleName", roleName));
    }

    /// <summary>
    /// Tạo Role mới - không có password (gọi Stored Procedure)
    /// </summary>
    public void CreateRole(string roleName)
    {
        ExecuteProcedure("SP_CREATE_ROLE",
            new OracleParameter("p_role_name", roleName)
        );
    }

    /// <summary>
    /// Tạo Role mới với password (gọi Stored Procedure)
    /// </summary>
    public void CreateRoleWithPassword(string roleName, string password)
    {
        ExecuteProcedure("SP_CREATE_ROLE_WITH_PASSWORD",
            new OracleParameter("p_role_name", roleName),
            new OracleParameter("p_password", password)
        );
    }

    /// <summary>
    /// Đổi password của Role (gọi Stored Procedure)
    /// </summary>
    public void ChangeRolePassword(string roleName, string newPassword)
    {
        ExecuteProcedure("SP_CHANGE_ROLE_PASSWORD",
            new OracleParameter("p_role_name", roleName),
            new OracleParameter("p_new_password", newPassword)
        );
    }

    /// <summary>
    /// Xóa password của Role (gọi Stored Procedure)
    /// </summary>
    public void RemoveRolePassword(string roleName)
    {
        ExecuteProcedure("SP_REMOVE_ROLE_PASSWORD",
            new OracleParameter("p_role_name", roleName)
        );
    }

    /// <summary>
    /// Xóa Role (gọi Stored Procedure)
    /// </summary>
    public void DeleteRole(string roleName)
    {
        ExecuteProcedure("SP_DELETE_ROLE",
            new OracleParameter("p_role_name", roleName)
        );
    }

    /// <summary>
    /// Kiểm tra Role tồn tại
    /// </summary>
    public bool RoleExists(string roleName)
    {
        const string sql = "SELECT COUNT(*) FROM DBA_ROLES WHERE UPPER(ROLE) = UPPER(:roleName)";
        return Exists(sql, new OracleParameter("roleName", roleName));
    }

    /// <summary>
    /// Lấy danh sách Users được gán Role này
    /// </summary>
    public DataTable GetRoleGrantees(string roleName)
    {
        const string sql = @"
            SELECT 
                GRANTEE,
                ADMIN_OPTION,
                DEFAULT_ROLE
            FROM DBA_ROLE_PRIVS
            WHERE UPPER(GRANTED_ROLE) = UPPER(:roleName)
            ORDER BY GRANTEE";

        return ExecuteQuery(sql, new OracleParameter("roleName", roleName));
    }

    /// <summary>
    /// Lấy danh sách System Privileges của Role
    /// </summary>
    public DataTable GetRoleSystemPrivileges(string roleName)
    {
        const string sql = @"
            SELECT 
                PRIVILEGE,
                ADMIN_OPTION
            FROM DBA_SYS_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:roleName)
            ORDER BY PRIVILEGE";

        return ExecuteQuery(sql, new OracleParameter("roleName", roleName));
    }

    /// <summary>
    /// Lấy danh sách Object Privileges của Role
    /// </summary>
    public DataTable GetRoleObjectPrivileges(string roleName)
    {
        const string sql = @"
            SELECT 
                OWNER,
                TABLE_NAME,
                PRIVILEGE,
                GRANTABLE
            FROM DBA_TAB_PRIVS
            WHERE UPPER(GRANTEE) = UPPER(:roleName)
            ORDER BY OWNER, TABLE_NAME, PRIVILEGE";

        return ExecuteQuery(sql, new OracleParameter("roleName", roleName));
    }
}
