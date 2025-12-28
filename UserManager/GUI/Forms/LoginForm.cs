namespace UserManager.GUI.Forms;

/// <summary>
/// Form Đăng nhập
/// </summary>
public partial class LoginForm : Form
{
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
        this.Size = new Size(400, 280);
        this.BackColor = Color.White;

        // Title Label
        var lblTitle = new Label
        {
            Text = "🔐 ĐĂNG NHẬP HỆ THỐNG",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(70, 20)
        };
        this.Controls.Add(lblTitle);

        // Username Label
        var lblUsername = new Label
        {
            Text = "Tên đăng nhập:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(50, 70),
            AutoSize = true
        };
        this.Controls.Add(lblUsername);

        // Username TextBox
        txtUsername = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(50, 95),
            Size = new Size(280, 30),
            PlaceholderText = "Nhập username Oracle..."
        };
        this.Controls.Add(txtUsername);

        // Password Label
        var lblPassword = new Label
        {
            Text = "Mật khẩu:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(50, 130),
            AutoSize = true
        };
        this.Controls.Add(lblPassword);

        // Password TextBox
        txtPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(50, 155),
            Size = new Size(280, 30),
            PasswordChar = '●',
            PlaceholderText = "Nhập mật khẩu..."
        };
        this.Controls.Add(txtPassword);

        // Login Button
        btnLogin = new Button
        {
            Text = "Đăng Nhập",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(50, 200),
            Size = new Size(135, 35),
            BackColor = Color.FromArgb(0, 102, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += BtnLogin_Click;
        this.Controls.Add(btnLogin);

        // Cancel Button
        btnCancel = new Button
        {
            Text = "Thoát",
            Font = new Font("Segoe UI", 11),
            Location = new Point(195, 200),
            Size = new Size(135, 35),
            BackColor = Color.FromArgb(220, 220, 220),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => Application.Exit();
        this.Controls.Add(btnCancel);

        // Accept button
        this.AcceptButton = btnLogin;

        // Focus username
        this.Load += (s, e) => txtUsername.Focus();
    }

    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnCancel = null!;

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
