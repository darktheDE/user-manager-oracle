using UserManager.DAL.Repositories;
using UserManager.Models;
using UserManager.Common.Constants;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho Role Management
/// </summary>
public class RoleService
{
    private readonly RoleRepository _roleRepo;
    private readonly PrivilegeRepository _privilegeRepo;

    public RoleService()
    {
        _roleRepo = new RoleRepository();
        _privilegeRepo = new PrivilegeRepository();
    }

    #region CRUD Operations

    /// <summary>
    /// Lấy danh sách tất cả Roles
    /// </summary>
    public DataTable GetAllRoles()
    {
        return _roleRepo.GetAllRoles();
    }

    /// <summary>
    /// Lấy thông tin chi tiết Role
    /// </summary>
    public RoleModel? GetRoleDetails(string roleName)
    {
        var dt = _roleRepo.GetRoleByName(roleName);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        var role = new RoleModel
        {
            RoleName = row["ROLE"].ToString()!,
            PasswordRequired = row["PASSWORD_REQUIRED"].ToString() == "YES",
            AuthenticationType = row["AUTHENTICATION_TYPE"]?.ToString()
        };

        // Load grantees
        var grantees = _roleRepo.GetRoleGrantees(roleName);
        role.Grantees = grantees.AsEnumerable()
            .Select(r => r["GRANTEE"].ToString()!)
            .ToList();

        return role;
    }

    /// <summary>
    /// Tạo Role mới (không có password)
    /// </summary>
    public void CreateRole(string roleName)
    {
        // Kiểm tra quyền CREATE ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.CREATE_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền tạo Role");
        }

        // Validate
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Tên Role không được để trống");

        // Kiểm tra tồn tại
        if (_roleRepo.RoleExists(roleName))
            throw new InvalidOperationException($"Role '{roleName}' đã tồn tại");

        _roleRepo.CreateRole(roleName);
    }

    /// <summary>
    /// Tạo Role mới với password
    /// </summary>
    public void CreateRoleWithPassword(string roleName, string password)
    {
        // Kiểm tra quyền CREATE ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.CREATE_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền tạo Role");
        }

        // Validate
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Tên Role không được để trống");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password không được để trống");

        // Kiểm tra tồn tại
        if (_roleRepo.RoleExists(roleName))
            throw new InvalidOperationException($"Role '{roleName}' đã tồn tại");

        _roleRepo.CreateRoleWithPassword(roleName, password);
    }

    /// <summary>
    /// Đổi password của Role
    /// </summary>
    public void ChangeRolePassword(string roleName, string newPassword)
    {
        // Kiểm tra quyền ALTER ANY ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Role");
        }

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password mới không được để trống");

        _roleRepo.ChangeRolePassword(roleName, newPassword);
    }

    /// <summary>
    /// Xóa password của Role
    /// </summary>
    public void RemoveRolePassword(string roleName)
    {
        // Kiểm tra quyền ALTER ANY ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Role");
        }

        _roleRepo.RemoveRolePassword(roleName);
    }

    /// <summary>
    /// Xóa Role
    /// </summary>
    public void DeleteRole(string roleName)
    {
        // Kiểm tra quyền DROP ANY ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.DROP_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xóa Role");
        }

        _roleRepo.DeleteRole(roleName);
    }

    #endregion

    #region Role Privileges

    /// <summary>
    /// Lấy System Privileges của Role
    /// </summary>
    public DataTable GetRoleSystemPrivileges(string roleName)
    {
        return _roleRepo.GetRoleSystemPrivileges(roleName);
    }

    /// <summary>
    /// Lấy Object Privileges của Role
    /// </summary>
    public DataTable GetRoleObjectPrivileges(string roleName)
    {
        return _roleRepo.GetRoleObjectPrivileges(roleName);
    }

    /// <summary>
    /// Lấy danh sách Users được gán Role
    /// </summary>
    public DataTable GetRoleGrantees(string roleName)
    {
        return _roleRepo.GetRoleGrantees(roleName);
    }

    #endregion

    #region Grant/Revoke Role

    /// <summary>
    /// Grant Role cho User
    /// </summary>
    public void GrantRoleToUser(string roleName, string username, bool withAdminOption = false)
    {
        // Kiểm tra quyền GRANT ANY ROLE
        if (!new AuthService().HasPrivilege(SystemPrivileges.GRANT_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền gán Role");
        }

        _privilegeRepo.GrantRole(roleName, username, withAdminOption);
    }

    /// <summary>
    /// Revoke Role từ User
    /// </summary>
    public void RevokeRoleFromUser(string roleName, string username)
    {
        // Kiểm tra quyền GRANT ANY ROLE hoặc ADMIN OPTION trên role đó
        if (!new AuthService().HasPrivilege(SystemPrivileges.GRANT_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền thu hồi Role");
        }

        _privilegeRepo.RevokeRole(roleName, username);
    }

    #endregion
}
