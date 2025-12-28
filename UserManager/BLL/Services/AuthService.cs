using UserManager.DAL;
using UserManager.DAL.Repositories;
using UserManager.Models;
using UserManager.Common.Constants;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý Authentication (Đăng nhập)
/// </summary>
public class AuthService
{
    private readonly PrivilegeRepository _privilegeRepo;
    private static SessionModel? _currentSession;

    public AuthService()
    {
        _privilegeRepo = new PrivilegeRepository();
    }

    /// <summary>
    /// Session hiện tại
    /// </summary>
    public static SessionModel? CurrentSession => _currentSession;

    /// <summary>
    /// Kiểm tra đã đăng nhập chưa
    /// </summary>
    public static bool IsLoggedIn => _currentSession != null;

    /// <summary>
    /// Kiểm tra có phải Admin không
    /// </summary>
    public static bool IsAdmin => _currentSession?.IsAdmin ?? false;

    /// <summary>
    /// Đăng nhập
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="password">Mật khẩu</param>
    /// <returns>Tuple (success, errorMessage)</returns>
    public (bool Success, string ErrorMessage) Login(string username, string password)
    {
        try
        {
            // Test kết nối với credentials
            var connManager = OracleConnectionManager.Instance;
            if (!connManager.TestConnection(username, password, out var errorMsg))
            {
                return (false, $"Đăng nhập thất bại: {errorMsg}");
            }

            // Lưu credentials vào connection manager
            connManager.SetCredentials(username, password);

            // Tạo session
            _currentSession = new SessionModel
            {
                Username = username.ToUpper(),
                LoginTime = DateTime.Now
            };

            // Load privileges và roles
            LoadSessionPrivileges();

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi đăng nhập: {ex.Message}");
        }
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    public void Logout()
    {
        _currentSession = null;
        OracleConnectionManager.Instance.ClearCredentials();
    }

    /// <summary>
    /// Load privileges của session hiện tại
    /// </summary>
    private void LoadSessionPrivileges()
    {
        if (_currentSession == null) return;

        try
        {
            // Lấy System Privileges
            var sysPrivs = _privilegeRepo.GetSystemPrivileges(_currentSession.Username);
            _currentSession.SystemPrivileges = sysPrivs.AsEnumerable()
                .Select(r => r["PRIVILEGE"].ToString()!)
                .ToList();

            // Lấy Roles
            var roles = _privilegeRepo.GetGrantedRoles(_currentSession.Username);
            _currentSession.Roles = roles.AsEnumerable()
                .Select(r => r["GRANTED_ROLE"].ToString()!)
                .ToList();

            // Kiểm tra xem có phải Admin không (có quyền DBA hoặc các quyền quản lý user)
            _currentSession.IsAdmin = CheckIsAdmin();
        }
        catch
        {
            // Nếu không query được System Catalog thì không phải admin
            _currentSession.IsAdmin = false;
        }
    }

    /// <summary>
    /// Kiểm tra user có phải Admin không
    /// </summary>
    private bool CheckIsAdmin()
    {
        if (_currentSession == null) return false;

        // Kiểm tra có role DBA không
        if (_currentSession.Roles.Contains("DBA"))
            return true;

        // Kiểm tra có các quyền quản lý User không
        var adminPrivileges = new[] { 
            SystemPrivileges.CREATE_USER, 
            SystemPrivileges.ALTER_USER, 
            SystemPrivileges.DROP_USER 
        };

        return adminPrivileges.Any(p => 
            _currentSession.SystemPrivileges.Contains(p) || 
            _privilegeRepo.HasSystemPrivilege(_currentSession.Username, p));
    }

    /// <summary>
    /// Kiểm tra user hiện tại có quyền cụ thể không
    /// </summary>
    public bool HasPrivilege(string privilege)
    {
        if (_currentSession == null) return false;

        // Nếu là Admin thì có tất cả quyền
        if (_currentSession.IsAdmin) return true;

        // Kiểm tra quyền trực tiếp
        if (_currentSession.SystemPrivileges.Contains(privilege))
            return true;

        // Kiểm tra qua Role
        return _privilegeRepo.HasSystemPrivilege(_currentSession.Username, privilege);
    }

    /// <summary>
    /// Đổi password của user hiện tại
    /// </summary>
    public (bool Success, string ErrorMessage) ChangeOwnPassword(string currentPassword, string newPassword)
    {
        if (_currentSession == null)
            return (false, "Chưa đăng nhập");

        try
        {
            // Verify current password
            var connManager = OracleConnectionManager.Instance;
            if (!connManager.TestConnection(_currentSession.Username, currentPassword, out _))
            {
                return (false, "Mật khẩu hiện tại không đúng");
            }

            // User tự đổi password của chính mình bằng ALTER USER ... IDENTIFIED BY ... REPLACE
            // Điều này tránh lỗi ORA-65066 trong CDB/PDB
            using var conn = connManager.GetConnection();
            var sql = $"ALTER USER \"{_currentSession.Username}\" IDENTIFIED BY \"{newPassword}\" REPLACE \"{currentPassword}\"";
            using var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(sql, conn);
            cmd.ExecuteNonQuery();

            // Update credentials
            connManager.SetCredentials(_currentSession.Username, newPassword);

            return (true, "Đổi mật khẩu thành công");
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi đổi mật khẩu: {ex.Message}");
        }
    }
}
