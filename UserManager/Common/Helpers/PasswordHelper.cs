using System.Security.Cryptography;
using System.Text;

namespace UserManager.Common.Helpers;

/// <summary>
/// Helper class để mã hóa và xử lý password
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// Mã hóa password bằng SHA256
    /// </summary>
    /// <param name="password">Password gốc</param>
    /// <returns>Password đã mã hóa (Base64)</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Mã hóa password bằng SHA256 với Salt
    /// </summary>
    /// <param name="password">Password gốc</param>
    /// <param name="salt">Salt value</param>
    /// <returns>Password đã mã hóa (Base64)</returns>
    public static string HashPasswordWithSalt(string password, string salt)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        var saltedPassword = password + salt;
        return HashPassword(saltedPassword);
    }

    /// <summary>
    /// Tạo Salt ngẫu nhiên
    /// </summary>
    /// <param name="length">Độ dài salt</param>
    /// <returns>Salt string</returns>
    public static string GenerateSalt(int length = 32)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Xác thực password
    /// </summary>
    /// <param name="inputPassword">Password nhập vào</param>
    /// <param name="storedHash">Hash đã lưu</param>
    /// <returns>True nếu khớp</returns>
    public static bool VerifyPassword(string inputPassword, string storedHash)
    {
        var inputHash = HashPassword(inputPassword);
        return inputHash == storedHash;
    }

    /// <summary>
    /// Xác thực password với Salt
    /// </summary>
    public static bool VerifyPasswordWithSalt(string inputPassword, string storedHash, string salt)
    {
        var inputHash = HashPasswordWithSalt(inputPassword, salt);
        return inputHash == storedHash;
    }

    /// <summary>
    /// Kiểm tra độ mạnh của password theo Oracle 23ai password policy
    /// </summary>
    /// <param name="password">Password cần kiểm tra</param>
    /// <param name="minLength">Độ dài tối thiểu</param>
    /// <param name="requireUppercase">Yêu cầu chữ hoa</param>
    /// <param name="requireLowercase">Yêu cầu chữ thường</param>
    /// <param name="requireDigit">Yêu cầu số</param>
    /// <param name="requireSpecial">Yêu cầu ký tự đặc biệt (Oracle 23ai yêu cầu mặc định)</param>
    /// <returns>Tuple (isValid, errorMessage)</returns>
    public static (bool IsValid, string ErrorMessage) ValidatePasswordStrength(
        string password,
        int minLength = 8,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireDigit = true,
        bool requireSpecial = true)  // Oracle 23ai yêu cầu ký tự đặc biệt
    {
        if (string.IsNullOrEmpty(password))
            return (false, "Mật khẩu không được để trống");

        if (password.Length < minLength)
            return (false, $"Mật khẩu phải có ít nhất {minLength} ký tự");

        if (requireUppercase && !password.Any(char.IsUpper))
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ HOA (A-Z)");

        if (requireLowercase && !password.Any(char.IsLower))
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ thường (a-z)");

        if (requireDigit && !password.Any(char.IsDigit))
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ số (0-9)");

        if (requireSpecial && !password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt (VD: @, #, $, !, %, etc.)");

        return (true, string.Empty);
    }

    /// <summary>
    /// Tạo gợi ý mật khẩu hợp lệ cho Oracle 23ai
    /// </summary>
    public static string GetPasswordHint()
    {
        return "Mật khẩu phải có:\n" +
               "• Ít nhất 8 ký tự\n" +
               "• Ít nhất 1 chữ HOA (A-Z)\n" +
               "• Ít nhất 1 chữ thường (a-z)\n" +
               "• Ít nhất 1 chữ số (0-9)\n" +
               "• Ít nhất 1 ký tự đặc biệt (@, #, $, !, %, etc.)";
    }
}
