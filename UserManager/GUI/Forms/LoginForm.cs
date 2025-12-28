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
        this.Size = new Size(450, 350);
        this.BackColor = AppTheme.CardBackground;

        // Left Panel (branding)
        var panelLeft = new Panel
        {
            Dock = DockStyle.Left,
            Width = 150,
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
        panelLeft.Controls.Add(lblBrand);
        
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
        panelLeft.Controls.Add(lblBrandText);
        
        this.Controls.Add(panelLeft);

        // Right Panel (login form)
        var panelRight = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(20)
        };

        // Title Label
        var lblTitle = new Label
        {
            Text = "Đăng Nhập",
            Font = AppTheme.FontLarge,
            ForeColor = AppTheme.TextTitle,
            AutoSize = true,
            Location = new Point(20, 30)
        };
        panelRight.Controls.Add(lblTitle);

        // Subtitle
        var lblSubtitle = new Label
        {
            Text = "Vui lòng nhập thông tin đăng nhập Oracle",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 60)
        };
        panelRight.Controls.Add(lblSubtitle);

        // Username Label
        var lblUsername = new Label
        {
            Text = "Tên đăng nhập",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(20, 100),
            AutoSize = true
        };
        panelRight.Controls.Add(lblUsername);

        // Username TextBox
        txtUsername = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(20, 125),
            Size = new Size(240, 30),
            PlaceholderText = "Nhập username Oracle..."
        };
        panelRight.Controls.Add(txtUsername);

        // Password Label
        var lblPassword = new Label
        {
            Text = "Mật khẩu",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(20, 165),
            AutoSize = true
        };
        panelRight.Controls.Add(lblPassword);

        // Password TextBox
        txtPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(20, 190),
            Size = new Size(240, 30),
            PasswordChar = '●',
            PlaceholderText = "Nhập mật khẩu..."
        };
        panelRight.Controls.Add(txtPassword);

        // Login Button
        btnLogin = new Button
        {
            Text = "Đăng Nhập",
            Font = AppTheme.FontBold,
            Location = new Point(20, 240),
            Size = new Size(115, 38),
            BackColor = AppTheme.PrimaryButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += BtnLogin_Click;
        panelRight.Controls.Add(btnLogin);

        // Cancel Button
        btnCancel = new Button
        {
            Text = "Thoát",
            Font = AppTheme.FontRegular,
            Location = new Point(145, 240),
            Size = new Size(115, 38),
            BackColor = AppTheme.ContentBackground,
            ForeColor = AppTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = AppTheme.CardBorder;
        btnCancel.Click += (s, e) => Application.Exit();
        panelRight.Controls.Add(btnCancel);

        this.Controls.Add(panelRight);

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
            btnLogin.Text = "Đang đăng nhập...";
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
