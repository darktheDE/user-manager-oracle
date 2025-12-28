using Oracle.ManagedDataAccess.Client;
using UserManager.Common.Helpers;

namespace UserManager.DAL;

/// <summary>
/// Base class quản lý kết nối Oracle Database
/// </summary>
public class OracleConnectionManager
{
    private static OracleConnectionManager? _instance;
    private static readonly object _lock = new();
    
    private string _connectionString;
    private string? _currentUsername;
    private string? _currentPassword;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static OracleConnectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new OracleConnectionManager();
                }
            }
            return _instance;
        }
    }

    private OracleConnectionManager()
    {
        _connectionString = ConfigHelper.GetConnectionString();
    }

    /// <summary>
    /// Username hiện tại đang đăng nhập
    /// </summary>
    public string? CurrentUsername => _currentUsername;

    /// <summary>
    /// Kiểm tra đã đăng nhập chưa
    /// </summary>
    public bool IsLoggedIn => !string.IsNullOrEmpty(_currentUsername);

    /// <summary>
    /// Cập nhật thông tin đăng nhập
    /// </summary>
    public void SetCredentials(string username, string password)
    {
        _currentUsername = username.ToUpper();
        _currentPassword = password;
        
        // Cập nhật connection string với user/password mới
        var builder = new OracleConnectionStringBuilder(_connectionString)
        {
            UserID = username,
            Password = password
        };
        _connectionString = builder.ConnectionString;
    }

    /// <summary>
    /// Xóa thông tin đăng nhập (logout)
    /// </summary>
    public void ClearCredentials()
    {
        _currentUsername = null;
        _currentPassword = null;
        _connectionString = ConfigHelper.GetConnectionString();
    }

    /// <summary>
    /// Tạo và mở kết nối mới
    /// </summary>
    public OracleConnection GetConnection()
    {
        var connection = new OracleConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Tạo kết nối với credentials cụ thể (dùng cho đăng nhập)
    /// </summary>
    public OracleConnection GetConnection(string username, string password)
    {
        var builder = new OracleConnectionStringBuilder(ConfigHelper.GetConnectionString())
        {
            UserID = username,
            Password = password
        };
        
        var connection = new OracleConnection(builder.ConnectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Test kết nối
    /// </summary>
    public bool TestConnection()
    {
        try
        {
            using var conn = GetConnection();
            return conn.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Test kết nối với credentials cụ thể
    /// </summary>
    public bool TestConnection(string username, string password, out string errorMessage)
    {
        errorMessage = string.Empty;
        try
        {
            using var conn = GetConnection(username, password);
            return conn.State == System.Data.ConnectionState.Open;
        }
        catch (OracleException ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}
