using System.Text.Json;

namespace UserManager.Common.Helpers;

/// <summary>
/// Helper class đọc cấu hình từ appsettings.json
/// </summary>
public static class ConfigHelper
{
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    private static Dictionary<string, object>? _config;

    /// <summary>
    /// Đọc toàn bộ cấu hình từ file
    /// </summary>
    private static Dictionary<string, object> LoadConfig()
    {
        if (_config != null)
            return _config;

        if (!File.Exists(ConfigPath))
            throw new FileNotFoundException($"Không tìm thấy file cấu hình: {ConfigPath}");

        var json = File.ReadAllText(ConfigPath);
        _config = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        return _config;
    }

    /// <summary>
    /// Lấy connection string Oracle
    /// </summary>
    public static string GetConnectionString()
    {
        var config = LoadConfig();
        if (config.TryGetValue("ConnectionStrings", out var connStrings))
        {
            var connDict = JsonSerializer.Deserialize<Dictionary<string, string>>(connStrings.ToString()!);
            if (connDict != null && connDict.TryGetValue("OracleConnection", out var oracleConn))
            {
                return oracleConn;
            }
        }
        throw new InvalidOperationException("Không tìm thấy cấu hình OracleConnection");
    }

    /// <summary>
    /// Lấy giá trị cấu hình từ AppSettings
    /// </summary>
    public static T? GetAppSetting<T>(string key)
    {
        var config = LoadConfig();
        if (config.TryGetValue("AppSettings", out var appSettings))
        {
            var settingsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(appSettings.ToString()!);
            if (settingsDict != null && settingsDict.TryGetValue(key, out var value))
            {
                return JsonSerializer.Deserialize<T>(value.GetRawText());
            }
        }
        return default;
    }

    /// <summary>
    /// Lấy thuật toán hash password
    /// </summary>
    public static string GetHashAlgorithm()
    {
        return GetAppSetting<string>("HashAlgorithm") ?? "SHA256";
    }

    /// <summary>
    /// Lấy thời gian timeout session (phút)
    /// </summary>
    public static int GetSessionTimeout()
    {
        return GetAppSetting<int>("SessionTimeoutMinutes");
    }

    /// <summary>
    /// Lấy số lần đăng nhập sai tối đa
    /// </summary>
    public static int GetMaxLoginAttempts()
    {
        return GetAppSetting<int>("MaxLoginAttempts");
    }

    /// <summary>
    /// Kiểm tra có bật audit log không
    /// </summary>
    public static bool IsAuditLogEnabled()
    {
        return GetAppSetting<bool>("EnableAuditLog");
    }

    /// <summary>
    /// Reload lại cấu hình từ file
    /// </summary>
    public static void ReloadConfig()
    {
        _config = null;
        LoadConfig();
    }
}
