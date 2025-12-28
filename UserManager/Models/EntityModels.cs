namespace UserManager.Models;

/// <summary>
/// Model đại diện cho Oracle User
/// </summary>
public class UserModel
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? AccountStatus { get; set; }
    public DateTime? LockDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? DefaultTablespace { get; set; }
    public string? TemporaryTablespace { get; set; }
    public string? Profile { get; set; }
    public string? Quota { get; set; }

    /// <summary>
    /// Thông tin cá nhân bổ sung (liên kết với bảng USER_INFO)
    /// </summary>
    public UserInfoModel? UserInfo { get; set; }

    /// <summary>
    /// Danh sách Roles được gán
    /// </summary>
    public List<RoleGrantModel> Roles { get; set; } = new();

    /// <summary>
    /// Danh sách Privileges
    /// </summary>
    public List<PrivilegeModel> Privileges { get; set; } = new();
}

/// <summary>
/// Model đại diện cho thông tin cá nhân bổ sung
/// </summary>
public class UserInfoModel
{
    public int UserInfoId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public string? ChucVu { get; set; }
    public string? PhongBan { get; set; }
    public string? MaNhanVien { get; set; }
    public string? AvatarPath { get; set; }
    public string? GhiChu { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Model đại diện cho Oracle Role
/// </summary>
public class RoleModel
{
    public string RoleName { get; set; } = string.Empty;
    public bool PasswordRequired { get; set; }
    public string? AuthenticationType { get; set; }
    public string? Password { get; set; }

    /// <summary>
    /// Danh sách Users được gán Role này
    /// </summary>
    public List<string> Grantees { get; set; } = new();

    /// <summary>
    /// Danh sách Privileges của Role
    /// </summary>
    public List<PrivilegeModel> Privileges { get; set; } = new();
}

/// <summary>
/// Model đại diện cho Role được gán
/// </summary>
public class RoleGrantModel
{
    public string RoleName { get; set; } = string.Empty;
    public bool AdminOption { get; set; }
    public bool DefaultRole { get; set; }
}

/// <summary>
/// Model đại diện cho Oracle Profile
/// </summary>
public class ProfileModel
{
    public string ProfileName { get; set; } = string.Empty;
    public string? SessionsPerUser { get; set; }
    public string? ConnectTime { get; set; }
    public string? IdleTime { get; set; }

    /// <summary>
    /// Danh sách Users được gán Profile này
    /// </summary>
    public List<string> Users { get; set; } = new();
}

/// <summary>
/// Model đại diện cho Privilege
/// </summary>
public class PrivilegeModel
{
    public string PrivilegeName { get; set; } = string.Empty;
    public string PrivilegeType { get; set; } = string.Empty; // SYSTEM hoặc OBJECT
    public string Source { get; set; } = string.Empty; // DIRECT hoặc ROLE
    public string? SourceRole { get; set; }
    public bool AdminOption { get; set; }
    public bool GrantOption { get; set; }

    // Cho Object Privilege
    public string? ObjectOwner { get; set; }
    public string? ObjectName { get; set; }
    public string? ColumnName { get; set; }
}

/// <summary>
/// Model đại diện cho Tablespace
/// </summary>
public class TablespaceModel
{
    public string TablespaceName { get; set; } = string.Empty;
    public int BlockSize { get; set; }
    public string? Status { get; set; }
    public string? Contents { get; set; } // PERMANENT, TEMPORARY, UNDO
    public decimal TotalMB { get; set; }
    public decimal UsedMB { get; set; }
    public decimal FreeMB { get; set; }
    public decimal UsedPercent { get; set; }
}

/// <summary>
/// Model đại diện cho Session của User đang đăng nhập
/// </summary>
public class SessionModel
{
    public string Username { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public bool IsAdmin { get; set; }
    public List<string> SystemPrivileges { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}
