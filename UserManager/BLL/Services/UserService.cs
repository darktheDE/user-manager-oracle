using UserManager.DAL.Repositories;
using UserManager.Models;
using UserManager.Common.Constants;
using UserManager.Common.Helpers;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho User Management
/// </summary>
public class UserService
{
    private readonly UserRepository _userRepo;
    private readonly UserInfoRepository _userInfoRepo;
    private readonly PrivilegeRepository _privilegeRepo;

    public UserService()
    {
        _userRepo = new UserRepository();
        _userInfoRepo = new UserInfoRepository();
        _privilegeRepo = new PrivilegeRepository();
    }

    #region CRUD Operations

    /// <summary>
    /// Lấy danh sách tất cả Users
    /// </summary>
    public DataTable GetAllUsers()
    {
        // Kiểm tra quyền
        if (!AuthService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xem danh sách người dùng");
        }

        return _userRepo.GetAllUsers();
    }

    /// <summary>
    /// Lấy thông tin User hiện tại (cho user thường)
    /// </summary>
    public DataTable GetCurrentUserInfo()
    {
        var username = AuthService.CurrentSession?.Username;
        if (string.IsNullOrEmpty(username))
            throw new InvalidOperationException("Chưa đăng nhập");

        return _userRepo.GetUserByUsername(username);
    }

    /// <summary>
    /// Lấy thông tin chi tiết User
    /// </summary>
    public UserModel? GetUserDetails(string username)
    {
        // Kiểm tra quyền (admin hoặc chính user đó)
        if (!AuthService.IsAdmin && 
            !string.Equals(username, AuthService.CurrentSession?.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xem thông tin người dùng này");
        }

        var dt = _userRepo.GetUserByUsername(username);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        var user = new UserModel
        {
            Username = row["USERNAME"].ToString()!,
            AccountStatus = row["ACCOUNT_STATUS"]?.ToString(),
            LockDate = row["LOCK_DATE"] == DBNull.Value ? null : Convert.ToDateTime(row["LOCK_DATE"]),
            CreatedDate = Convert.ToDateTime(row["CREATED_DATE"]),
            DefaultTablespace = row["DEFAULT_TABLESPACE"]?.ToString(),
            TemporaryTablespace = row["TEMPORARY_TABLESPACE"]?.ToString(),
            Profile = row["PROFILE"]?.ToString()
        };

        // Load Quota từ DBA_TS_QUOTAS
        var quotaDt = _userRepo.GetUserQuotas(username);
        if (quotaDt.Rows.Count > 0)
        {
            // Lấy quota của default tablespace
            foreach (System.Data.DataRow qRow in quotaDt.Rows)
            {
                if (qRow["TABLESPACE_NAME"]?.ToString() == user.DefaultTablespace)
                {
                    var maxBytes = qRow["MAX_BYTES"];
                    if (maxBytes == DBNull.Value || Convert.ToInt64(maxBytes) == -1)
                    {
                        user.Quota = "UNLIMITED";
                    }
                    else
                    {
                        var bytes = Convert.ToInt64(maxBytes);
                        if (bytes >= 1073741824) // 1GB
                            user.Quota = $"{bytes / 1073741824}G";
                        else if (bytes >= 1048576) // 1MB
                            user.Quota = $"{bytes / 1048576}M";
                        else
                            user.Quota = $"{bytes}";
                    }
                    break;
                }
            }
        }

        // Load thông tin bổ sung
        user.UserInfo = _userInfoRepo.GetByUsername(username);

        return user;
    }

    /// <summary>
    /// Tạo User mới
    /// </summary>
    public void CreateUser(UserModel user, UserInfoModel? userInfo = null)
    {
        // Kiểm tra quyền CREATE USER
        if (!new AuthService().HasPrivilege(SystemPrivileges.CREATE_USER))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền tạo người dùng");
        }

        // Validate
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Username không được để trống");

        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentException("Password không được để trống");

        // Validate password strength
        var (isValid, errorMsg) = PasswordHelper.ValidatePasswordStrength(user.Password);
        if (!isValid)
            throw new ArgumentException(errorMsg);

        // Kiểm tra tồn tại
        if (_userRepo.UserExists(user.Username))
            throw new InvalidOperationException($"User '{user.Username}' đã tồn tại");

        // Tạo Oracle User
        _userRepo.CreateUser(user);

        // Tạo thông tin bổ sung nếu có (không bắt buộc, nếu lỗi thì bỏ qua)
        if (userInfo != null)
        {
            try
            {
                userInfo.Username = user.Username;
                _userInfoRepo.Insert(userInfo);
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw - Oracle User đã tạo thành công
                System.Diagnostics.Debug.WriteLine($"Warning: Không thể lưu thông tin bổ sung: {ex.Message}");
                // Có thể hiển thị warning cho user
            }
        }
    }

    /// <summary>
    /// Cập nhật User
    /// </summary>
    public void UpdateUser(UserModel user, UserInfoModel? userInfo = null)
    {
        // Kiểm tra quyền ALTER USER
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_USER))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền sửa thông tin người dùng");
        }

        // Validate
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Username không được để trống");

        // Validate password nếu có thay đổi
        if (!string.IsNullOrEmpty(user.Password))
        {
            var (isValid, errorMsg) = PasswordHelper.ValidatePasswordStrength(user.Password);
            if (!isValid)
                throw new ArgumentException(errorMsg);
        }

        // Cập nhật Oracle User
        _userRepo.UpdateUser(user);

        // Cập nhật thông tin bổ sung
        if (userInfo != null)
        {
            userInfo.Username = user.Username;
            if (_userInfoRepo.Exists(user.Username))
                _userInfoRepo.Update(userInfo);
            else
                _userInfoRepo.Insert(userInfo);
        }
    }

    /// <summary>
    /// Xóa User
    /// </summary>
    public void DeleteUser(string username)
    {
        // Kiểm tra quyền DROP USER
        if (!new AuthService().HasPrivilege(SystemPrivileges.DROP_USER))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xóa người dùng");
        }

        // Không cho phép tự xóa chính mình
        if (string.Equals(username, AuthService.CurrentSession?.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Không thể xóa tài khoản đang đăng nhập");
        }

        // Xóa thông tin bổ sung trước
        if (_userInfoRepo.Exists(username))
            _userInfoRepo.Delete(username);

        // Xóa Oracle User
        _userRepo.DeleteUser(username);
    }

    #endregion

    #region Lock/Unlock

    /// <summary>
    /// Lock User
    /// </summary>
    public void LockUser(string username)
    {
        // Kiểm tra quyền ALTER USER
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_USER))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền khóa người dùng");
        }

        // Không cho phép tự khóa chính mình
        if (string.Equals(username, AuthService.CurrentSession?.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Không thể khóa tài khoản đang đăng nhập");
        }

        _userRepo.LockUser(username);
    }

    /// <summary>
    /// Unlock User
    /// </summary>
    public void UnlockUser(string username)
    {
        // Kiểm tra quyền ALTER USER
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_USER))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền mở khóa người dùng");
        }

        _userRepo.UnlockUser(username);
    }

    #endregion

    #region Privileges & Roles

    /// <summary>
    /// Lấy danh sách quyền của User
    /// </summary>
    public DataTable GetUserPrivileges(string username)
    {
        return _privilegeRepo.GetAllUserPrivileges(username);
    }

    /// <summary>
    /// Lấy danh sách Roles của User
    /// </summary>
    public DataTable GetUserRoles(string username)
    {
        return _privilegeRepo.GetGrantedRoles(username);
    }

    /// <summary>
    /// Lấy Quotas của User
    /// </summary>
    public DataTable GetUserQuotas(string username)
    {
        return _userRepo.GetUserQuotas(username);
    }

    #endregion
}
