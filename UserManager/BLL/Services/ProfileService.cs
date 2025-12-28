using UserManager.DAL.Repositories;
using UserManager.Models;
using UserManager.Common.Constants;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho Profile Management
/// </summary>
public class ProfileService
{
    private readonly ProfileRepository _profileRepo;

    public ProfileService()
    {
        _profileRepo = new ProfileRepository();
    }

    #region CRUD Operations

    /// <summary>
    /// Lấy danh sách tất cả Profiles
    /// </summary>
    public DataTable GetAllProfiles()
    {
        return _profileRepo.GetAllProfiles();
    }

    /// <summary>
    /// Lấy thông tin chi tiết Profile
    /// </summary>
    public ProfileModel? GetProfileDetails(string profileName)
    {
        var dt = _profileRepo.GetProfileResources(profileName);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        var profile = new ProfileModel
        {
            ProfileName = row["PROFILE"].ToString()!,
            SessionsPerUser = row["SESSIONS_PER_USER"]?.ToString(),
            ConnectTime = row["CONNECT_TIME"]?.ToString(),
            IdleTime = row["IDLE_TIME"]?.ToString()
        };

        // Load users
        var users = _profileRepo.GetProfileUsers(profileName);
        profile.Users = users.AsEnumerable()
            .Select(r => r["USERNAME"].ToString()!)
            .ToList();

        return profile;
    }

    /// <summary>
    /// Lấy tất cả resources của Profile
    /// </summary>
    public DataTable GetProfileResources(string profileName)
    {
        return _profileRepo.GetProfileDetails(profileName);
    }

    /// <summary>
    /// Tạo Profile mới
    /// </summary>
    public void CreateProfile(ProfileModel profile)
    {
        // Kiểm tra quyền CREATE PROFILE
        if (!new AuthService().HasPrivilege(SystemPrivileges.CREATE_PROFILE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền tạo Profile");
        }

        // Validate
        if (string.IsNullOrWhiteSpace(profile.ProfileName))
            throw new ArgumentException("Tên Profile không được để trống");

        // Kiểm tra tồn tại
        if (_profileRepo.ProfileExists(profile.ProfileName))
            throw new InvalidOperationException($"Profile '{profile.ProfileName}' đã tồn tại");

        // Validate resource values
        ValidateResourceValue(profile.SessionsPerUser, "SESSIONS_PER_USER");
        ValidateResourceValue(profile.ConnectTime, "CONNECT_TIME");
        ValidateResourceValue(profile.IdleTime, "IDLE_TIME");

        _profileRepo.CreateProfile(
            profile.ProfileName,
            profile.SessionsPerUser,
            profile.ConnectTime,
            profile.IdleTime
        );
    }

    /// <summary>
    /// Cập nhật Profile
    /// </summary>
    public void UpdateProfile(ProfileModel profile)
    {
        // Kiểm tra quyền ALTER PROFILE
        if (!new AuthService().HasPrivilege(SystemPrivileges.ALTER_PROFILE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Profile");
        }

        // Validate resource values
        ValidateResourceValue(profile.SessionsPerUser, "SESSIONS_PER_USER");
        ValidateResourceValue(profile.ConnectTime, "CONNECT_TIME");
        ValidateResourceValue(profile.IdleTime, "IDLE_TIME");

        _profileRepo.UpdateProfile(
            profile.ProfileName,
            profile.SessionsPerUser,
            profile.ConnectTime,
            profile.IdleTime
        );
    }

    /// <summary>
    /// Xóa Profile
    /// </summary>
    public void DeleteProfile(string profileName)
    {
        // Kiểm tra quyền DROP PROFILE
        if (!new AuthService().HasPrivilege(SystemPrivileges.DROP_PROFILE))
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xóa Profile");
        }

        // Không cho phép xóa DEFAULT profile
        if (profileName.ToUpper() == "DEFAULT")
        {
            throw new InvalidOperationException("Không thể xóa Profile DEFAULT");
        }

        _profileRepo.DeleteProfile(profileName);
    }

    /// <summary>
    /// Lấy danh sách Users được gán Profile
    /// </summary>
    public DataTable GetProfileUsers(string profileName)
    {
        return _profileRepo.GetProfileUsers(profileName);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate giá trị resource
    /// </summary>
    private void ValidateResourceValue(string? value, string resourceName)
    {
        if (string.IsNullOrEmpty(value)) return;

        var upperValue = value.ToUpper().Trim();

        // Các giá trị hợp lệ: UNLIMITED, DEFAULT, hoặc số nguyên dương
        if (upperValue == ProfileResources.UNLIMITED || upperValue == ProfileResources.DEFAULT)
            return;

        if (!int.TryParse(value, out var intValue) || intValue < 0)
        {
            throw new ArgumentException($"Giá trị của {resourceName} phải là UNLIMITED, DEFAULT hoặc số nguyên dương");
        }
    }

    #endregion
}
