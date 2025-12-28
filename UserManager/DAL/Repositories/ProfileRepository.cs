using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý Oracle Profiles
/// </summary>
public class ProfileRepository : BaseRepository
{
    /// <summary>
    /// Lấy danh sách tất cả Profiles
    /// </summary>
    public DataTable GetAllProfiles()
    {
        const string sql = @"
            SELECT DISTINCT PROFILE 
            FROM DBA_PROFILES 
            ORDER BY PROFILE";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy thông tin chi tiết Profile (các resources)
    /// </summary>
    public DataTable GetProfileDetails(string profileName)
    {
        const string sql = @"
            SELECT 
                PROFILE,
                RESOURCE_NAME,
                RESOURCE_TYPE,
                LIMIT
            FROM DBA_PROFILES
            WHERE UPPER(PROFILE) = UPPER(:profileName)
            ORDER BY RESOURCE_TYPE, RESOURCE_NAME";

        return ExecuteQuery(sql, new OracleParameter("profileName", profileName));
    }

    /// <summary>
    /// Lấy Profile với các resource cần quản lý
    /// </summary>
    public DataTable GetProfileResources(string profileName)
    {
        const string sql = @"
            SELECT 
                PROFILE,
                MAX(CASE WHEN RESOURCE_NAME = 'SESSIONS_PER_USER' THEN LIMIT END) AS SESSIONS_PER_USER,
                MAX(CASE WHEN RESOURCE_NAME = 'CONNECT_TIME' THEN LIMIT END) AS CONNECT_TIME,
                MAX(CASE WHEN RESOURCE_NAME = 'IDLE_TIME' THEN LIMIT END) AS IDLE_TIME
            FROM DBA_PROFILES
            WHERE UPPER(PROFILE) = UPPER(:profileName)
            GROUP BY PROFILE";

        return ExecuteQuery(sql, new OracleParameter("profileName", profileName));
    }

    /// <summary>
    /// Tạo Profile mới (gọi Stored Procedure)
    /// </summary>
    public void CreateProfile(string profileName, string? sessionsPerUser = null, string? connectTime = null, string? idleTime = null)
    {
        ExecuteProcedure("SP_CREATE_PROFILE",
            new OracleParameter("p_profile_name", profileName),
            new OracleParameter("p_sessions_per_user", sessionsPerUser ?? "UNLIMITED"),
            new OracleParameter("p_connect_time", connectTime ?? "UNLIMITED"),
            new OracleParameter("p_idle_time", idleTime ?? "UNLIMITED")
        );
    }

    /// <summary>
    /// Cập nhật Profile (gọi Stored Procedure)
    /// </summary>
    public void UpdateProfile(string profileName, string? sessionsPerUser = null, string? connectTime = null, string? idleTime = null)
    {
        ExecuteProcedure("SP_UPDATE_PROFILE",
            new OracleParameter("p_profile_name", profileName),
            new OracleParameter("p_sessions_per_user", string.IsNullOrEmpty(sessionsPerUser) ? (object)DBNull.Value : sessionsPerUser),
            new OracleParameter("p_connect_time", string.IsNullOrEmpty(connectTime) ? (object)DBNull.Value : connectTime),
            new OracleParameter("p_idle_time", string.IsNullOrEmpty(idleTime) ? (object)DBNull.Value : idleTime)
        );
    }

    /// <summary>
    /// Xóa Profile (gọi Stored Procedure)
    /// </summary>
    public void DeleteProfile(string profileName)
    {
        ExecuteProcedure("SP_DELETE_PROFILE",
            new OracleParameter("p_profile_name", profileName)
        );
    }

    /// <summary>
    /// Kiểm tra Profile tồn tại
    /// </summary>
    public bool ProfileExists(string profileName)
    {
        const string sql = "SELECT COUNT(DISTINCT PROFILE) FROM DBA_PROFILES WHERE UPPER(PROFILE) = UPPER(:profileName)";
        return Exists(sql, new OracleParameter("profileName", profileName));
    }

    /// <summary>
    /// Lấy danh sách Users được gán Profile này
    /// </summary>
    public DataTable GetProfileUsers(string profileName)
    {
        const string sql = @"
            SELECT 
                USERNAME,
                ACCOUNT_STATUS,
                CREATED
            FROM DBA_USERS
            WHERE UPPER(PROFILE) = UPPER(:profileName)
              AND ORACLE_MAINTAINED = 'N'
            ORDER BY USERNAME";

        return ExecuteQuery(sql, new OracleParameter("profileName", profileName));
    }
}
