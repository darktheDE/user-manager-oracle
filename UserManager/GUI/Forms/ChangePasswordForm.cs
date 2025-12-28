using UserManager.BLL.Services;
using UserManager.Common.Helpers;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form đổi mật khẩu
/// </summary>
public partial class ChangePasswordForm : Form
{
    private readonly AuthService _authService;
    private readonly UserService _userService;
    private readonly string? _targetUsername;
    private readonly bool _isChangingOwnPassword;

    private TextBox txtCurrentPassword = null!;
    private TextBox txtNewPassword = null!;
    private TextBox txtConfirmPassword = null!;

    /// <summary>
    /// Constructor - đổi mật khẩu của chính mình
    /// </summary>
    public ChangePasswordForm() : this(null) { }

    /// <summary>
    /// Constructor - đổi mật khẩu cho user khác (Admin only)
    /// </summary>
    public ChangePasswordForm(string? targetUsername)
    {
        InitializeComponent();
        _authService = new AuthService();
        _userService = new UserService();
        _targetUsername = targetUsername;
        _isChangingOwnPassword = string.IsNullOrEmpty(targetUsername) || 
            string.Equals(targetUsername, AuthService.CurrentSession?.Username, StringComparison.OrdinalIgnoreCase);
        
        SetupForm();
    }

    private void SetupForm()
    {
        this.Text = _isChangingOwnPassword ? "🔑 Đổi Mật Khẩu" : $"🔑 Đổi Mật Khẩu: {_targetUsername}";
        this.Size = new Size(450, _isChangingOwnPassword ? 380 : 340);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30)
        };
        this.Controls.Add(panel);

        int y = 20;

        // Title
        var lblTitle = new Label
        {
            Text = "🔑 ĐỔI MẬT KHẨU",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(30, y)
        };
        panel.Controls.Add(lblTitle);
        y += 50;

        // Current Password (only for changing own password)
        if (_isChangingOwnPassword)
        {
            var lblCurrent = new Label
            {
                Text = "Mật khẩu hiện tại:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, y),
                AutoSize = true
            };
            panel.Controls.Add(lblCurrent);
            y += 25;

            txtCurrentPassword = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, y),
                Width = 350,
                PasswordChar = '●'
            };
            panel.Controls.Add(txtCurrentPassword);
            y += 40;
        }

        // New Password
        var lblNew = new Label
        {
            Text = "Mật khẩu mới:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, y),
            AutoSize = true
        };
        panel.Controls.Add(lblNew);
        y += 25;

        txtNewPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, y),
            Width = 350,
            PasswordChar = '●'
        };
        panel.Controls.Add(txtNewPassword);
        y += 40;

        // Confirm Password
        var lblConfirm = new Label
        {
            Text = "Xác nhận mật khẩu mới:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, y),
            AutoSize = true
        };
        panel.Controls.Add(lblConfirm);
        y += 25;

        txtConfirmPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, y),
            Width = 350,
            PasswordChar = '●'
        };
        panel.Controls.Add(txtConfirmPassword);
        y += 50;

        // Buttons
        var btnSave = new Button
        {
            Text = "💾 Lưu",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(120, 40),
            Location = new Point(100, y),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;
        panel.Controls.Add(btnSave);

        var btnCancel = new Button
        {
            Text = "❌ Hủy",
            Font = new Font("Segoe UI", 11),
            Size = new Size(120, 40),
            Location = new Point(230, y),
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => this.Close();
        panel.Controls.Add(btnCancel);
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            // Validate
            if (_isChangingOwnPassword && string.IsNullOrEmpty(txtCurrentPassword.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập mật khẩu hiện tại");
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtNewPassword.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập mật khẩu mới");
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageHelper.ShowWarning("Mật khẩu xác nhận không khớp");
                txtConfirmPassword.Focus();
                return;
            }

            // Validate password strength
            var (isValid, errorMsg) = PasswordHelper.ValidatePasswordStrength(txtNewPassword.Text);
            if (!isValid)
            {
                MessageHelper.ShowWarning(errorMsg);
                txtNewPassword.Focus();
                return;
            }

            if (_isChangingOwnPassword)
            {
                // Đổi mật khẩu của chính mình
                var (success, message) = _authService.ChangeOwnPassword(txtCurrentPassword.Text, txtNewPassword.Text);
                if (success)
                {
                    MessageHelper.ShowSuccess("Đổi mật khẩu thành công!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageHelper.ShowError(message);
                }
            }
            else
            {
                // Admin đổi mật khẩu cho user khác
                var user = new Models.UserModel
                {
                    Username = _targetUsername!,
                    Password = txtNewPassword.Text
                };
                _userService.UpdateUser(user);
                MessageHelper.ShowSuccess($"Đổi mật khẩu cho '{_targetUsername}' thành công!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }
}
