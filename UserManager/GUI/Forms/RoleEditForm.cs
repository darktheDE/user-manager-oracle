using UserManager.BLL.Services;
using UserManager.Common.Helpers;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form thêm/sửa Role
/// </summary>
public partial class RoleEditForm : Form
{
    private readonly RoleService _roleService;
    private readonly string? _editRoleName;
    private readonly bool _isEditMode;

    private TextBox txtRoleName = null!;
    private CheckBox chkHasPassword = null!;
    private TextBox txtPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private Label lblPasswordInfo = null!;

    public RoleEditForm(string? editRoleName = null)
    {
        InitializeComponent();
        _roleService = new RoleService();
        _editRoleName = editRoleName;
        _isEditMode = !string.IsNullOrEmpty(editRoleName);
        
        SetupForm();
        
        if (_isEditMode)
        {
            LoadRoleData();
        }
    }

    private void SetupForm()
    {
        this.Text = _isEditMode ? $"Sửa Role: {_editRoleName}" : "Thêm Role Mới";
        this.Size = new Size(450, 420);
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
            Text = _isEditMode ? "🎭 SỬA ROLE" : "🎭 TẠO ROLE MỚI",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(30, y)
        };
        panel.Controls.Add(lblTitle);
        y += 50;

        // Role Name
        var lblRoleName = new Label
        {
            Text = "Tên Role:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, y),
            AutoSize = true
        };
        panel.Controls.Add(lblRoleName);
        y += 25;

        txtRoleName = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, y),
            Width = 350,
            CharacterCasing = CharacterCasing.Upper,
            Enabled = !_isEditMode
        };
        panel.Controls.Add(txtRoleName);
        y += 40;

        // Has Password checkbox
        chkHasPassword = new CheckBox
        {
            Text = "🔑 Role có mật khẩu",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, y),
            AutoSize = true
        };
        chkHasPassword.CheckedChanged += (s, e) => TogglePasswordFields();
        panel.Controls.Add(chkHasPassword);
        y += 35;

        // Password
        var lblPassword = new Label
        {
            Text = "Mật khẩu:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, y),
            AutoSize = true
        };
        panel.Controls.Add(lblPassword);
        y += 25;

        txtPassword = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(30, y),
            Width = 350,
            PasswordChar = '●',
            Enabled = false
        };
        panel.Controls.Add(txtPassword);
        y += 35;

        // Confirm Password
        var lblConfirm = new Label
        {
            Text = "Xác nhận mật khẩu:",
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
            PasswordChar = '●',
            Enabled = false
        };
        panel.Controls.Add(txtConfirmPassword);
        y += 40;

        // Password info label
        lblPasswordInfo = new Label
        {
            Text = _isEditMode ? "💡 Để trống nếu không muốn đổi mật khẩu" : "",
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.Gray,
            Location = new Point(30, y),
            AutoSize = true,
            Visible = _isEditMode
        };
        panel.Controls.Add(lblPasswordInfo);

        // Buttons
        var btnSave = new Button
        {
            Text = "💾 Lưu",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(120, 40),
            Location = new Point(100, 310),
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
            Location = new Point(230, 310),
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => this.Close();
        panel.Controls.Add(btnCancel);
    }

    private void TogglePasswordFields()
    {
        txtPassword.Enabled = chkHasPassword.Checked;
        txtConfirmPassword.Enabled = chkHasPassword.Checked;
        
        if (!chkHasPassword.Checked)
        {
            txtPassword.Clear();
            txtConfirmPassword.Clear();
        }
    }

    private void LoadRoleData()
    {
        if (string.IsNullOrEmpty(_editRoleName)) return;

        try
        {
            var role = _roleService.GetRoleDetails(_editRoleName);
            if (role == null)
            {
                MessageHelper.ShowError("Không tìm thấy Role");
                this.Close();
                return;
            }

            txtRoleName.Text = role.RoleName;
            chkHasPassword.Checked = role.PasswordRequired;
            TogglePasswordFields();
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            var roleName = txtRoleName.Text.Trim().ToUpper();

            // Validate
            if (string.IsNullOrWhiteSpace(roleName))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên Role");
                txtRoleName.Focus();
                return;
            }

            if (chkHasPassword.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập mật khẩu cho Role");
                    txtPassword.Focus();
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageHelper.ShowWarning("Mật khẩu xác nhận không khớp");
                    txtConfirmPassword.Focus();
                    return;
                }
            }

            if (_isEditMode)
            {
                // Edit mode: chỉ đổi password
                if (chkHasPassword.Checked && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    _roleService.ChangeRolePassword(roleName, txtPassword.Text);
                    MessageHelper.ShowSuccess("Cập nhật mật khẩu Role thành công!");
                }
                else if (!chkHasPassword.Checked)
                {
                    _roleService.RemoveRolePassword(roleName);
                    MessageHelper.ShowSuccess("Đã xóa mật khẩu của Role!");
                }
            }
            else
            {
                // Create mode
                if (chkHasPassword.Checked)
                {
                    _roleService.CreateRoleWithPassword(roleName, txtPassword.Text);
                }
                else
                {
                    _roleService.CreateRole(roleName);
                }
                MessageHelper.ShowSuccess("Tạo Role thành công!");
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }
}
