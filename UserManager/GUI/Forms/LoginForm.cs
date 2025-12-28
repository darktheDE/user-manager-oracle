using UserManager.GUI.Core;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form Đăng nhập với theme mới
/// </summary>
public partial class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnCancel = null!;

    public LoginForm()
    {
        InitializeComponent();
        SetupForm();
    }

    private void SetupForm()
    {
        // Form settings
        this.Text = "Đăng Nhập - UserManager";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Size = new Size(500, 320);
        this.BackColor = AppTheme.CardBackground;

        // Main content panel (login form) - LEFT side
        var panelContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(30, 20, 20, 20)
        };

        // Title Label
        var lblTitle = new Label
        {
            Text = "Đăng Nhập",
            Font = AppTheme.FontLarge,
            ForeColor = AppTheme.TextTitle,
            AutoSize = true,
            Location = new Point(30, 20)
        };
        panelContent.Controls.Add(lblTitle);

        // Subtitle
        var lblSubtitle = new Label
        {
            Text = "Vui lòng nhập thông tin đăng nhập Oracle",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(30, 50)
        };
        panelContent.Controls.Add(lblSubtitle);

        // Username Label
        var lblUsername = new Label
        {
            Text = "Tên đăng nhập",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(30, 85),
            AutoSize = true
        };
        panelContent.Controls.Add(lblUsername);

        // Username TextBox
        txtUsername = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, 108),
            Size = new Size(220, 30),
            PlaceholderText = "Nhập username Oracle..."
        };
        panelContent.Controls.Add(txtUsername);

        // Password Label
        var lblPassword = new Label
        {
            Text = "Mật khẩu",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(30, 145),
            AutoSize = true
        };
        panelContent.Controls.Add(lblPassword);

        // Password TextBox
        txtPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, 168),
            Size = new Size(220, 30),
            PasswordChar = '●',
            PlaceholderText = "Nhập mật khẩu..."
        };
        panelContent.Controls.Add(txtPassword);

        // Login Button
        btnLogin = new Button
        {
            Text = "Đăng Nhập",
            Font = AppTheme.FontBold,
            Location = new Point(30, 215),
            Size = new Size(105, 38),
            BackColor = AppTheme.PrimaryButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += BtnLogin_Click;
        panelContent.Controls.Add(btnLogin);

        // Cancel Button
        btnCancel = new Button
        {
            Text = "Thoát",
            Font = AppTheme.FontRegular,
            Location = new Point(145, 215),
            Size = new Size(105, 38),
            BackColor = AppTheme.ContentBackground,
            ForeColor = AppTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = AppTheme.CardBorder;
        btnCancel.Click += (s, e) => Application.Exit();
        panelContent.Controls.Add(btnCancel);

        this.Controls.Add(panelContent);

        // Branding Panel - RIGHT side
        var panelBrand = new Panel
        {
            Dock = DockStyle.Right,
            Width = 180,
            BackColor = AppTheme.SidebarBackground
        };

        var lblBrand = new Label
        {
            Text = "🗄️",
            Font = new Font("Segoe UI", 48),
            ForeColor = AppTheme.SidebarText,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };
        panelBrand.Controls.Add(lblBrand);
        
        var lblBrandText = new Label
        {
            Text = "User\nManager",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = AppTheme.SidebarText,
            AutoSize = false,
            TextAlign = ContentAlignment.TopCenter,
            Dock = DockStyle.Bottom,
            Height = 50
        };
        panelBrand.Controls.Add(lblBrandText);
        
        this.Controls.Add(panelBrand);

        // Accept button
        this.AcceptButton = btnLogin;

        // Focus username
        this.Load += (s, e) => txtUsername.Focus();
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        var username = txtUsername.Text.Trim();
        var password = txtPassword.Text;

        // Validate input
        if (string.IsNullOrEmpty(username))
        {
            MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtUsername.Focus();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPassword.Focus();
            return;
        }

        // Attempt login
        try
        {
            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xử lý...";
            Application.DoEvents();

            var authService = new BLL.Services.AuthService();
            var (success, errorMessage) = authService.Login(username, password);

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(errorMessage, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Đăng Nhập";
        }
    }
}
