namespace UserManager.Common.Constants;

/// <summary>
/// Các hằng số System Privileges trong Oracle
/// </summary>
public static class SystemPrivileges
{
    // Profile Privileges
    public const string CREATE_PROFILE = "CREATE PROFILE";
    public const string ALTER_PROFILE = "ALTER PROFILE";
    public const string DROP_PROFILE = "DROP PROFILE";

    // Role Privileges
    public const string CREATE_ROLE = "CREATE ROLE";
    public const string ALTER_ANY_ROLE = "ALTER ANY ROLE";
    public const string DROP_ANY_ROLE = "DROP ANY ROLE";
    public const string GRANT_ANY_ROLE = "GRANT ANY ROLE";

    // Session Privileges
    public const string CREATE_SESSION = "CREATE SESSION";

    // Table (ANY) Privileges
    public const string CREATE_ANY_TABLE = "CREATE ANY TABLE";
    public const string ALTER_ANY_TABLE = "ALTER ANY TABLE";
    public const string DROP_ANY_TABLE = "DROP ANY TABLE";
    public const string SELECT_ANY_TABLE = "SELECT ANY TABLE";
    public const string DELETE_ANY_TABLE = "DELETE ANY TABLE";
    public const string INSERT_ANY_TABLE = "INSERT ANY TABLE";
    public const string UPDATE_ANY_TABLE = "UPDATE ANY TABLE";

    // Table (Own) Privileges
    public const string CREATE_TABLE = "CREATE TABLE";

    // User Privileges
    public const string CREATE_USER = "CREATE USER";
    public const string ALTER_USER = "ALTER USER";
    public const string DROP_USER = "DROP USER";

    /// <summary>
    /// Danh sách tất cả System Privileges
    /// </summary>
    public static readonly string[] All = new[]
    {
        // Profile
        CREATE_PROFILE, ALTER_PROFILE, DROP_PROFILE,
        // Role
        CREATE_ROLE, ALTER_ANY_ROLE, DROP_ANY_ROLE, GRANT_ANY_ROLE,
        // Session
        CREATE_SESSION,
        // Table (ANY)
        CREATE_ANY_TABLE, ALTER_ANY_TABLE, DROP_ANY_TABLE,
        SELECT_ANY_TABLE, DELETE_ANY_TABLE, INSERT_ANY_TABLE, UPDATE_ANY_TABLE,
        // Table (Own)
        CREATE_TABLE,
        // User
        CREATE_USER, ALTER_USER, DROP_USER
    };

    /// <summary>
    /// Nhóm theo loại
    /// </summary>
    public static readonly Dictionary<string, string[]> ByCategory = new()
    {
        ["Profile"] = new[] { CREATE_PROFILE, ALTER_PROFILE, DROP_PROFILE },
        ["Role"] = new[] { CREATE_ROLE, ALTER_ANY_ROLE, DROP_ANY_ROLE, GRANT_ANY_ROLE },
        ["Session"] = new[] { CREATE_SESSION },
        ["Table (ANY)"] = new[] { CREATE_ANY_TABLE, ALTER_ANY_TABLE, DROP_ANY_TABLE, SELECT_ANY_TABLE, DELETE_ANY_TABLE, INSERT_ANY_TABLE, UPDATE_ANY_TABLE },
        ["Table (Own)"] = new[] { CREATE_TABLE },
        ["User"] = new[] { CREATE_USER, ALTER_USER, DROP_USER }
    };
}

/// <summary>
/// Các hằng số Object Privileges trong Oracle
/// </summary>
public static class ObjectPrivileges
{
    // Table Privileges
    public const string SELECT = "SELECT";
    public const string INSERT = "INSERT";
    public const string UPDATE = "UPDATE";
    public const string DELETE = "DELETE";

    /// <summary>
    /// Danh sách Object Privileges trên Table
    /// </summary>
    public static readonly string[] OnTable = new[] { SELECT, INSERT, UPDATE, DELETE };

    /// <summary>
    /// Danh sách Object Privileges trên Column
    /// </summary>
    public static readonly string[] OnColumn = new[] { SELECT, INSERT };
}

/// <summary>
/// Các hằng số cho Account Status
/// </summary>
public static class AccountStatus
{
    public const string OPEN = "OPEN";
    public const string LOCKED = "LOCKED";
    public const string EXPIRED = "EXPIRED";
    public const string EXPIRED_AND_LOCKED = "EXPIRED & LOCKED";

    public static readonly string[] All = new[] { OPEN, LOCKED, EXPIRED, EXPIRED_AND_LOCKED };
}

/// <summary>
/// Các hằng số cho Profile Resources
/// </summary>
public static class ProfileResources
{
    public const string SESSIONS_PER_USER = "SESSIONS_PER_USER";
    public const string CONNECT_TIME = "CONNECT_TIME";
    public const string IDLE_TIME = "IDLE_TIME";

    public const string UNLIMITED = "UNLIMITED";
    public const string DEFAULT = "DEFAULT";

    public static readonly string[] All = new[] { SESSIONS_PER_USER, CONNECT_TIME, IDLE_TIME };
}

/// <summary>
/// Các hằng số cho Action Types (Audit Log)
/// </summary>
public static class ActionTypes
{
    public const string LOGIN = "LOGIN";
    public const string LOGOUT = "LOGOUT";
    public const string CREATE = "CREATE";
    public const string UPDATE = "UPDATE";
    public const string DELETE = "DELETE";
    public const string GRANT = "GRANT";
    public const string REVOKE = "REVOKE";
    public const string LOCK = "LOCK";
    public const string UNLOCK = "UNLOCK";
    public const string VIEW = "VIEW";
}

/// <summary>
/// Các hằng số cho Object Types (Audit Log)
/// </summary>
public static class ObjectTypes
{
    public const string USER = "USER";
    public const string ROLE = "ROLE";
    public const string PROFILE = "PROFILE";
    public const string TABLE = "TABLE";
    public const string PRIVILEGE = "PRIVILEGE";
}
