using UserManager.DAL.Repositories;
using UserManager.Common.Constants;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho Privilege Management (Grant/Revoke)
/// </summary>
public class PrivilegeService
{
    private readonly PrivilegeRepository _privilegeRepo;

    public PrivilegeService()
    {
        _privilegeRepo = new PrivilegeRepository();
    }

    #region System Privileges

    /// <summary>
    /// Lấy tất cả System Privileges đã được gán
    /// </summary>
    public DataTable GetAllSystemPrivileges()
    {
        return _privilegeRepo.GetAllSystemPrivileges();
    }

    /// <summary>
    /// Lấy System Privileges của một User/Role
    /// </summary>
    public DataTable GetSystemPrivileges(string grantee)
    {
        return _privilegeRepo.GetSystemPrivileges(grantee);
    }

    /// <summary>
    /// Grant System Privilege
    /// </summary>
    public void GrantSystemPrivilege(string privilege, string grantee, bool withAdminOption = false)
    {
        // Kiểm tra có quyền grant không
        // User phải có Admin Option trên privilege đó hoặc là DBA
        if (!AuthService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền cấp System Privilege");
        }

        _privilegeRepo.GrantSystemPrivilege(privilege, grantee, withAdminOption);
    }

    /// <summary>
    /// Revoke System Privilege
    /// </summary>
    public void RevokeSystemPrivilege(string privilege, string grantee)
    {
        if (!AuthService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền thu hồi System Privilege");
        }

        // Không cho phép revoke từ chính mình
        if (string.Equals(grantee, AuthService.CurrentSession?.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Không thể thu hồi quyền từ tài khoản đang đăng nhập");
        }

        _privilegeRepo.RevokeSystemPrivilege(privilege, grantee);
    }

    #endregion

    #region Object Privileges

    /// <summary>
    /// Lấy tất cả Object Privileges đã được gán
    /// </summary>
    public DataTable GetAllObjectPrivileges()
    {
        return _privilegeRepo.GetAllObjectPrivileges();
    }

    /// <summary>
    /// Lấy Object Privileges của một User/Role
    /// </summary>
    public DataTable GetObjectPrivileges(string grantee)
    {
        return _privilegeRepo.GetObjectPrivileges(grantee);
    }

    /// <summary>
    /// Grant Object Privilege trên Table
    /// </summary>
    public void GrantObjectPrivilege(string privilege, string owner, string objectName, string grantee, bool withGrantOption = false)
    {
        // Kiểm tra privilege hợp lệ
        if (!ObjectPrivileges.OnTable.Contains(privilege.ToUpper()))
        {
            throw new ArgumentException($"Privilege '{privilege}' không hợp lệ cho Table. Các giá trị hợp lệ: {string.Join(", ", ObjectPrivileges.OnTable)}");
        }

        _privilegeRepo.GrantObjectPrivilege(privilege, owner, objectName, grantee, withGrantOption);
    }

    /// <summary>
    /// Revoke Object Privilege trên Table
    /// </summary>
    public void RevokeObjectPrivilege(string privilege, string owner, string objectName, string grantee)
    {
        _privilegeRepo.RevokeObjectPrivilege(privilege, owner, objectName, grantee);
    }

    #endregion

    #region Column Privileges

    /// <summary>
    /// Lấy Column Privileges của một User/Role
    /// </summary>
    public DataTable GetColumnPrivileges(string grantee)
    {
        return _privilegeRepo.GetColumnPrivileges(grantee);
    }

    /// <summary>
    /// Grant Column Privilege
    /// </summary>
    public void GrantColumnPrivilege(string privilege, string owner, string tableName, string columnName, string grantee)
    {
        // Kiểm tra privilege hợp lệ cho Column
        if (!ObjectPrivileges.OnColumn.Contains(privilege.ToUpper()))
        {
            throw new ArgumentException($"Privilege '{privilege}' không hợp lệ cho Column. Các giá trị hợp lệ: {string.Join(", ", ObjectPrivileges.OnColumn)}");
        }

        _privilegeRepo.GrantColumnPrivilege(privilege, owner, tableName, columnName, grantee);
    }

    /// <summary>
    /// Revoke Column Privilege
    /// </summary>
    public void RevokeColumnPrivilege(string privilege, string owner, string tableName, string columnName, string grantee)
    {
        _privilegeRepo.RevokeColumnPrivilege(privilege, owner, tableName, columnName, grantee);
    }

    #endregion

    #region Role Grants

    /// <summary>
    /// Lấy danh sách Roles được gán cho User/Role
    /// </summary>
    public DataTable GetGrantedRoles(string grantee)
    {
        return _privilegeRepo.GetGrantedRoles(grantee);
    }

    /// <summary>
    /// Grant Role
    /// </summary>
    public void GrantRole(string roleName, string grantee, bool withAdminOption = false)
    {
        if (!new AuthService().HasPrivilege(SystemPrivileges.GRANT_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền gán Role");
        }

        _privilegeRepo.GrantRole(roleName, grantee, withAdminOption);
    }

    /// <summary>
    /// Revoke Role
    /// </summary>
    public void RevokeRole(string roleName, string grantee)
    {
        if (!new AuthService().HasPrivilege(SystemPrivileges.GRANT_ANY_ROLE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền thu hồi Role");
        }

        _privilegeRepo.RevokeRole(roleName, grantee);
    }

    #endregion

    #region Check Privileges

    /// <summary>
    /// Kiểm tra User có System Privilege không
    /// </summary>
    public bool HasSystemPrivilege(string username, string privilege)
    {
        return _privilegeRepo.HasSystemPrivilege(username, privilege);
    }

    /// <summary>
    /// Lấy tất cả quyền của User (cả trực tiếp và qua Role)
    /// </summary>
    public DataTable GetAllUserPrivileges(string username)
    {
        return _privilegeRepo.GetAllUserPrivileges(username);
    }

    #endregion

    #region Available Privileges

    /// <summary>
    /// Lấy danh sách System Privileges có thể grant
    /// </summary>
    public string[] GetAvailableSystemPrivileges()
    {
        return SystemPrivileges.All;
    }

    /// <summary>
    /// Lấy System Privileges theo nhóm
    /// </summary>
    public Dictionary<string, string[]> GetSystemPrivilegesByCategory()
    {
        return SystemPrivileges.ByCategory;
    }

    /// <summary>
    /// Lấy danh sách Object Privileges có thể grant trên Table
    /// </summary>
    public string[] GetTablePrivileges()
    {
        return ObjectPrivileges.OnTable;
    }

    /// <summary>
    /// Lấy danh sách Object Privileges có thể grant trên Column
    /// </summary>
    public string[] GetColumnPrivilegeTypes()
    {
        return ObjectPrivileges.OnColumn;
    }

    #endregion
}
