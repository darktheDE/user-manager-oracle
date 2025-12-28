using UserManager.GUI.Forms;

namespace UserManager;

static class Program
{
    /// <summary>
    /// Entry point của ứng dụng
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Vòng lặp cho phép đăng nhập lại sau khi logout
        while (true)
        {
            // Hiển thị form đăng nhập
            using var loginForm = new LoginForm();
            var loginResult = loginForm.ShowDialog();

            if (loginResult != DialogResult.OK)
            {
                // Người dùng đóng form đăng nhập hoặc nhấn Cancel
                break;
            }

            // Đăng nhập thành công, hiển thị MainForm
            using var mainForm = new MainForm();
            var mainResult = mainForm.ShowDialog();

            if (mainResult != DialogResult.Retry)
            {
                // Người dùng đóng ứng dụng
                break;
            }

            // mainResult == DialogResult.Retry nghĩa là người dùng logout
            // Quay lại vòng lặp để hiển thị lại form đăng nhập
        }
    }
}