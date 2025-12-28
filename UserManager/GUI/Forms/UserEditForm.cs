using UserManager.BLL.Services;
using UserManager.Models;
using UserManager.Common.Helpers;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form thêm/sửa User
/// </summary>
public partial class UserEditForm : Form
{
    private readonly UserService _userService;
    private readonly TablespaceService _tablespaceService;
    private readonly ProfileService _profileService;
    private readonly RoleService _roleService;
    private readonly string? _editUsername;
    private readonly bool _isEditMode;

    // Controls
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private ComboBox cboDefaultTablespace = null!;
    private ComboBox cboTempTablespace = null!;
    private ComboBox cboProfile = null!;
    private ComboBox cboQuota = null!;
    private ComboBox cboRole = null!;
    private CheckBox chkLocked = null!;
    
    // Thông tin cá nhân
    private TextBox txtHoTen = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private ComboBox cboGioiTinh = null!;
    private TextBox txtDiaChi = null!;
    private TextBox txtSoDienThoai = null!;
    private TextBox txtEmail = null!;
    private TextBox txtChucVu = null!;
    private TextBox txtPhongBan = null!;
    private TextBox txtMaNhanVien = null!;

    public UserEditForm(string? editUsername = null)
    {
        InitializeComponent();
        _userService = new UserService();
        _tablespaceService = new TablespaceService();
        _profileService = new ProfileService();
        _roleService = new RoleService();
        _editUsername = editUsername;
        _isEditMode = !string.IsNullOrEmpty(editUsername);
        
        SetupForm();
        LoadComboBoxData();
        
        if (_isEditMode)
        {
            LoadUserData();
        }
    }

    private void SetupForm()
    {
        this.Text = _isEditMode ? $"Sửa User: {_editUsername}" : "Thêm User Mới";
        this.Size = new Size(700, 650);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        // TabControl
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };
        this.Controls.Add(tabControl);

        // Tab 1: Thông tin Oracle User
        var tabOracle = new TabPage("🔐 Tài khoản Oracle");
        tabControl.TabPages.Add(tabOracle);
        SetupOracleTab(tabOracle);

        // Tab 2: Thông tin cá nhân
        var tabInfo = new TabPage("👤 Thông tin cá nhân");
        tabControl.TabPages.Add(tabInfo);
        SetupInfoTab(tabInfo);

        // Button Panel
        var panelButtons = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.FromArgb(248, 248, 248)
        };
        this.Controls.Add(panelButtons);

        var btnSave = new Button
        {
            Text = "💾 Lưu",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(120, 40),
            Location = new Point(panelButtons.Width / 2 - 130, 10),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;
        panelButtons.Controls.Add(btnSave);

        var btnCancel = new Button
        {
            Text = "❌ Hủy",
            Font = new Font("Segoe UI", 11),
            Size = new Size(120, 40),
            Location = new Point(panelButtons.Width / 2 + 10, 10),
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => this.Close();
        panelButtons.Controls.Add(btnCancel);

        panelButtons.BringToFront();
    }

    private void SetupOracleTab(TabPage tab)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tab.Controls.Add(panel);

        int y = 10;
        int labelWidth = 150;
        int inputWidth = 250;

        // Username
        AddLabel(panel, "Username:", 10, y);
        txtUsername = AddTextBox(panel, labelWidth, y, inputWidth);
        txtUsername.CharacterCasing = CharacterCasing.Upper;
        txtUsername.Enabled = !_isEditMode;
        y += 40;

        // Password
        AddLabel(panel, "Mật khẩu:", 10, y);
        txtPassword = AddTextBox(panel, labelWidth, y, inputWidth);
        txtPassword.PasswordChar = '●';
        txtPassword.PlaceholderText = _isEditMode ? "(Để trống nếu không đổi)" : "Nhập mật khẩu...";
        y += 40;

        // Confirm Password
        AddLabel(panel, "Xác nhận mật khẩu:", 10, y);
        txtConfirmPassword = AddTextBox(panel, labelWidth, y, inputWidth);
        txtConfirmPassword.PasswordChar = '●';
        y += 30;

        // Password hint (Oracle 23ai requirements)
        var lblPasswordHint = new Label
        {
            Text = "💡 Mật khẩu: ít nhất 8 ký tự, 1 HOA, 1 thường, 1 số, 1 ký tự đặc biệt (@#$!%)",
            Font = new Font("Segoe UI", 8, FontStyle.Italic),
            ForeColor = Color.Gray,
            Location = new Point(labelWidth, y),
            AutoSize = true
        };
        panel.Controls.Add(lblPasswordHint);
        y += 25;

        // Default Tablespace
        AddLabel(panel, "Default Tablespace:", 10, y);
        cboDefaultTablespace = AddComboBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Temp Tablespace
        AddLabel(panel, "Temp Tablespace:", 10, y);
        cboTempTablespace = AddComboBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Profile
        AddLabel(panel, "Profile:", 10, y);
        cboProfile = AddComboBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Quota
        AddLabel(panel, "Quota:", 10, y);
        cboQuota = AddComboBox(panel, labelWidth, y, inputWidth);
        cboQuota.Items.AddRange(new object[] { "UNLIMITED", "10M", "50M", "100M", "500M", "1G", "5G" });
        cboQuota.SelectedIndex = 0;
        y += 40;

        // Role (Grant cho User sau khi tạo)
        AddLabel(panel, "Role:", 10, y);
        cboRole = AddComboBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Locked
        chkLocked = new CheckBox
        {
            Text = "🔒 Khóa tài khoản",
            Font = new Font("Segoe UI", 10),
            Location = new Point(labelWidth, y),
            AutoSize = true
        };
        panel.Controls.Add(chkLocked);
    }

    private void SetupInfoTab(TabPage tab)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        tab.Controls.Add(panel);

        int y = 10;
        int labelWidth = 130;
        int inputWidth = 280;

        // Họ tên
        AddLabel(panel, "Họ tên (*):", 10, y);
        txtHoTen = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Ngày sinh
        AddLabel(panel, "Ngày sinh:", 10, y);
        dtpNgaySinh = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = new Font("Segoe UI", 10),
            Location = new Point(labelWidth, y),
            Width = inputWidth,
            ShowCheckBox = true,
            Checked = false
        };
        panel.Controls.Add(dtpNgaySinh);
        y += 40;

        // Giới tính
        AddLabel(panel, "Giới tính:", 10, y);
        cboGioiTinh = AddComboBox(panel, labelWidth, y, inputWidth);
        cboGioiTinh.Items.AddRange(new object[] { "", "Nam", "Nữ", "Khác" });
        y += 40;

        // Địa chỉ
        AddLabel(panel, "Địa chỉ:", 10, y);
        txtDiaChi = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Số điện thoại
        AddLabel(panel, "Số điện thoại:", 10, y);
        txtSoDienThoai = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Email
        AddLabel(panel, "Email:", 10, y);
        txtEmail = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Chức vụ
        AddLabel(panel, "Chức vụ:", 10, y);
        txtChucVu = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Phòng ban
        AddLabel(panel, "Phòng ban:", 10, y);
        txtPhongBan = AddTextBox(panel, labelWidth, y, inputWidth);
        y += 40;

        // Mã nhân viên
        AddLabel(panel, "Mã nhân viên:", 10, y);
        txtMaNhanVien = AddTextBox(panel, labelWidth, y, inputWidth);
    }

    private void AddLabel(Panel panel, string text, int x, int y)
    {
        var label = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10),
            Location = new Point(x, y + 3),
            AutoSize = true
        };
        panel.Controls.Add(label);
    }

    private TextBox AddTextBox(Panel panel, int x, int y, int width)
    {
        var textBox = new TextBox
        {
            Font = new Font("Segoe UI", 10),
            Location = new Point(x, y),
            Width = width
        };
        panel.Controls.Add(textBox);
        return textBox;
    }

    private ComboBox AddComboBox(Panel panel, int x, int y, int width)
    {
        var comboBox = new ComboBox
        {
            Font = new Font("Segoe UI", 10),
            Location = new Point(x, y),
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        panel.Controls.Add(comboBox);
        return comboBox;
    }

    private void LoadComboBoxData()
    {
        try
        {
            // Load Tablespaces
            var permTablespaces = _tablespaceService.GetPermanentTablespaceNames();
            cboDefaultTablespace.Items.AddRange(permTablespaces.ToArray());
            if (cboDefaultTablespace.Items.Count > 0)
            {
                var usersIndex = permTablespaces.FindIndex(t => t == "USERS");
                cboDefaultTablespace.SelectedIndex = usersIndex >= 0 ? usersIndex : 0;
            }

            var tempTablespaces = _tablespaceService.GetTemporaryTablespaceNames();
            cboTempTablespace.Items.AddRange(tempTablespaces.ToArray());
            if (cboTempTablespace.Items.Count > 0)
            {
                var tempIndex = tempTablespaces.FindIndex(t => t == "TEMP");
                cboTempTablespace.SelectedIndex = tempIndex >= 0 ? tempIndex : 0;
            }

            // Load Profiles
            var profiles = _profileService.GetAllProfiles();
            foreach (System.Data.DataRow row in profiles.Rows)
            {
                cboProfile.Items.Add(row["PROFILE"].ToString()!);
            }
            if (cboProfile.Items.Count > 0)
            {
                var defaultIndex = cboProfile.Items.IndexOf("DEFAULT");
                cboProfile.SelectedIndex = defaultIndex >= 0 ? defaultIndex : 0;
            }

            // Load Roles
            cboRole.Items.Add("(Không gán Role)");  // Option mặc định
            var roles = _roleService.GetAllRoles();
            foreach (System.Data.DataRow row in roles.Rows)
            {
                cboRole.Items.Add(row["ROLE"].ToString()!);
            }
            cboRole.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private void LoadUserData()
    {
        if (string.IsNullOrEmpty(_editUsername)) return;

        try
        {
            var user = _userService.GetUserDetails(_editUsername);
            if (user == null)
            {
                MessageHelper.ShowError("Không tìm thấy thông tin User");
                this.Close();
                return;
            }

            txtUsername.Text = user.Username;
            
            if (!string.IsNullOrEmpty(user.DefaultTablespace))
                cboDefaultTablespace.SelectedItem = user.DefaultTablespace;
            
            if (!string.IsNullOrEmpty(user.TemporaryTablespace))
                cboTempTablespace.SelectedItem = user.TemporaryTablespace;
            
            if (!string.IsNullOrEmpty(user.Profile))
                cboProfile.SelectedItem = user.Profile;

            // Load Quota
            if (!string.IsNullOrEmpty(user.Quota))
            {
                // Tìm item phù hợp trong combobox
                var quotaValue = user.Quota.ToUpper();
                for (int i = 0; i < cboQuota.Items.Count; i++)
                {
                    if (cboQuota.Items[i].ToString()?.ToUpper() == quotaValue)
                    {
                        cboQuota.SelectedIndex = i;
                        break;
                    }
                }
            }

            chkLocked.Checked = user.AccountStatus?.Contains("LOCK") == true;

            // Load thông tin cá nhân
            if (user.UserInfo != null)
            {
                txtHoTen.Text = user.UserInfo.HoTen;
                if (user.UserInfo.NgaySinh.HasValue)
                {
                    dtpNgaySinh.Checked = true;
                    dtpNgaySinh.Value = user.UserInfo.NgaySinh.Value;
                }
                cboGioiTinh.SelectedItem = user.UserInfo.GioiTinh ?? "";
                txtDiaChi.Text = user.UserInfo.DiaChi;
                txtSoDienThoai.Text = user.UserInfo.SoDienThoai;
                txtEmail.Text = user.UserInfo.Email;
                txtChucVu.Text = user.UserInfo.ChucVu;
                txtPhongBan.Text = user.UserInfo.PhongBan;
                txtMaNhanVien.Text = user.UserInfo.MaNhanVien;
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu User: {ex.Message}");
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            // Validate
            if (!_isEditMode && string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập Username");
                txtUsername.Focus();
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập Mật khẩu");
                txtPassword.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(txtPassword.Text) && txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageHelper.ShowWarning("Mật khẩu xác nhận không khớp");
                txtConfirmPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập Họ tên");
                txtHoTen.Focus();
                return;
            }

            // Build UserModel
            var user = new UserModel
            {
                Username = txtUsername.Text.Trim().ToUpper(),
                Password = string.IsNullOrEmpty(txtPassword.Text) ? null : txtPassword.Text,
                DefaultTablespace = cboDefaultTablespace.SelectedItem?.ToString(),
                TemporaryTablespace = cboTempTablespace.SelectedItem?.ToString(),
                Profile = cboProfile.SelectedItem?.ToString(),
                Quota = cboQuota.SelectedItem?.ToString(),
                AccountStatus = chkLocked.Checked ? "LOCKED" : "OPEN"
            };

            // Build UserInfoModel
            var userInfo = new UserInfoModel
            {
                Username = user.Username,
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Checked ? dtpNgaySinh.Value : null,
                GioiTinh = cboGioiTinh.SelectedItem?.ToString(),
                DiaChi = txtDiaChi.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim(),
                PhongBan = txtPhongBan.Text.Trim(),
                MaNhanVien = txtMaNhanVien.Text.Trim()
            };

            if (_isEditMode)
            {
                _userService.UpdateUser(user, userInfo);
                
                // Xử lý Lock/Unlock account
                if (chkLocked.Checked)
                {
                    _userService.LockUser(user.Username);
                }
                else
                {
                    _userService.UnlockUser(user.Username);
                }
                
                MessageHelper.ShowSuccess("Cập nhật User thành công!");
            }
            else
            {
                _userService.CreateUser(user, userInfo);
                
                // Grant CREATE SESSION để user có thể đăng nhập
                var privilegeService = new PrivilegeService();
                privilegeService.GrantSystemPrivilege("CREATE SESSION", user.Username, false);
                
                // Grant Role nếu được chọn
                var selectedRole = cboRole.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedRole) && selectedRole != "(Không gán Role)")
                {
                    privilegeService.GrantRole(selectedRole, user.Username, false);
                }
                
                MessageHelper.ShowSuccess("Tạo User thành công!");
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
