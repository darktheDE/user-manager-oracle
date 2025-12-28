namespace UserManager.Common.Helpers;

/// <summary>
/// Helper class hiển thị thông báo cho người dùng
/// </summary>
public static class MessageHelper
{
    /// <summary>
    /// Hiển thị thông báo thành công
    /// </summary>
    public static void ShowSuccess(string message, string title = "Thành công")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Hiển thị thông báo lỗi
    /// </summary>
    public static void ShowError(string message, string title = "Lỗi")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Hiển thị cảnh báo
    /// </summary>
    public static void ShowWarning(string message, string title = "Cảnh báo")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /// <summary>
    /// Hiển thị hộp thoại xác nhận
    /// </summary>
    /// <returns>True nếu người dùng chọn Yes</returns>
    public static bool ShowConfirm(string message, string title = "Xác nhận")
    {
        var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        return result == DialogResult.Yes;
    }

    /// <summary>
    /// Hiển thị hộp thoại xác nhận với 3 lựa chọn
    /// </summary>
    public static DialogResult ShowConfirmWithCancel(string message, string title = "Xác nhận")
    {
        return MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
    }

    /// <summary>
    /// Hiển thị thông báo thông tin
    /// </summary>
    public static void ShowInfo(string message, string title = "Thông tin")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
