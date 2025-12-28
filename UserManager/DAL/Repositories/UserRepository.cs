using Oracle.ManagedDataAccess.Client;
using System.Data;
using UserManager.Models;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý Oracle Users
/// </summary>
public class UserRepository : BaseRepository
{
    /// <summary>
    /// Lấy danh sách tất cả Users (không bao gồm Oracle system users)
    /// </summary>
    public DataTable GetAllUsers()
    {
        const string sql = @"
            SELECT 
                USERNAME,
                ACCOUNT_STATUS,
                LOCK_DATE,
                CREATED AS CREATED_DATE,
                DEFAULT_TABLESPACE,
                TEMPORARY_TABLESPACE,
                PROFILE
            FROM DBA_USERS
            WHERE ORACLE_MAINTAINED = 'N'
            ORDER BY USERNAME";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy thông tin User theo Username
    /// </summary>
    public DataTable GetUserByUsername(string username)
    {
        const string sql = @"
            SELECT 
                USERNAME,
                ACCOUNT_STATUS,
                LOCK_DATE,
                CREATED AS CREATED_DATE,
                DEFAULT_TABLESPACE,
                TEMPORARY_TABLESPACE,
                PROFILE
            FROM DBA_USERS
            WHERE UPPER(USERNAME) = UPPER(:username)";

        return ExecuteQuery(sql, new OracleParameter("username", username));
    }

    /// <summary>
    /// Tạo User mới (gọi Stored Procedure)
    /// </summary>
    public void CreateUser(UserModel user)
    {
        ExecuteProcedure("SP_CREATE_USER",
            new OracleParameter("p_username", user.Username),
            new OracleParameter("p_password", user.Password),
            new OracleParameter("p_default_ts", user.DefaultTablespace ?? "USERS"),
            new OracleParameter("p_temp_ts", user.TemporaryTablespace ?? "TEMP"),
            new OracleParameter("p_profile", user.Profile ?? "DEFAULT"),
            new OracleParameter("p_quota", user.Quota ?? "UNLIMITED"),
            new OracleParameter("p_account_lock", user.AccountStatus?.ToUpper() == "LOCKED" ? 1 : 0)
        );
    }

    /// <summary>
    /// Cập nhật thông tin User (gọi Stored Procedure)
    /// </summary>
    public void UpdateUser(UserModel user)
    {
        ExecuteProcedure("SP_UPDATE_USER",
            new OracleParameter("p_username", user.Username),
            new OracleParameter("p_password", string.IsNullOrEmpty(user.Password) ? (object)DBNull.Value : user.Password),
            new OracleParameter("p_default_ts", string.IsNullOrEmpty(user.DefaultTablespace) ? (object)DBNull.Value : user.DefaultTablespace),
            new OracleParameter("p_temp_ts", string.IsNullOrEmpty(user.TemporaryTablespace) ? (object)DBNull.Value : user.TemporaryTablespace),
            new OracleParameter("p_profile", string.IsNullOrEmpty(user.Profile) ? (object)DBNull.Value : user.Profile),
            new OracleParameter("p_quota", string.IsNullOrEmpty(user.Quota) ? (object)DBNull.Value : user.Quota),
            new OracleParameter("p_quota_tablespace", string.IsNullOrEmpty(user.DefaultTablespace) ? (object)DBNull.Value : user.DefaultTablespace)
        );
    }

    /// <summary>
    /// Xóa User (gọi Stored Procedure)
    /// </summary>
    public void DeleteUser(string username)
    {
        ExecuteProcedure("SP_DELETE_USER",
            new OracleParameter("p_username", username)
        );
    }

    /// <summary>
    /// Lock User (gọi Stored Procedure)
    /// </summary>
    public void LockUser(string username)
    {
        ExecuteProcedure("SP_LOCK_USER",
            new OracleParameter("p_username", username)
        );
    }

    /// <summary>
    /// Unlock User (gọi Stored Procedure)
    /// </summary>
    public void UnlockUser(string username)
    {
        ExecuteProcedure("SP_UNLOCK_USER",
            new OracleParameter("p_username", username)
        );
    }

    /// <summary>
    /// Đổi password User (gọi Stored Procedure)
    /// </summary>
    public void ChangePassword(string username, string newPassword)
    {
        ExecuteProcedure("SP_CHANGE_PASSWORD",
            new OracleParameter("p_username", username),
            new OracleParameter("p_new_password", newPassword)
        );
    }

    /// <summary>
    /// Kiểm tra User tồn tại
    /// </summary>
    public bool UserExists(string username)
    {
        const string sql = "SELECT COUNT(*) FROM DBA_USERS WHERE UPPER(USERNAME) = UPPER(:username)";
        return Exists(sql, new OracleParameter("username", username));
    }

    /// <summary>
    /// Lấy Quota của User trên các Tablespace
    /// </summary>
    public DataTable GetUserQuotas(string username)
    {
        const string sql = @"
            SELECT 
                TABLESPACE_NAME,
                BYTES AS USED_BYTES,
                MAX_BYTES
            FROM DBA_TS_QUOTAS
            WHERE UPPER(USERNAME) = UPPER(:username)
            ORDER BY TABLESPACE_NAME";

        return ExecuteQuery(sql, new OracleParameter("username", username));
    }
}
