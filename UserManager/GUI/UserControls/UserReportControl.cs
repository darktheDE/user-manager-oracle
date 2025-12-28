using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị báo cáo thông tin đầy đủ của User
/// </summary>
public partial class UserReportControl : UserControl
{
    private readonly UserService _userService;
    private readonly PrivilegeService _privilegeService;

    private ComboBox cboUsers = null!;
    private DataGridView dgvUserInfo = null!;
    private DataGridView dgvRoles = null!;
    private DataGridView dgvPrivileges = null!;
    private DataGridView dgvQuotas = null!;

    public UserReportControl()
    {
        InitializeComponent();
        _userService = new UserService();
        _privilegeService = new PrivilegeService();
        SetupUI();
        LoadUsers();
    }

    private void SetupUI()
    {
        this.BackColor = Color.White;
        this.Padding = new Padding(10);

        // Header Panel
        var panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.White
        };

        // Title
        var lblTitle = new Label
        {
            Text = "📊 BÁO CÁO THÔNG TIN USER ĐẦY ĐỦ",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(10, 10)
        };
        panelHeader.Controls.Add(lblTitle);

        // User selector
        var lblUser = new Label
        {
            Text = "Chọn User:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(10, 50),
            AutoSize = true
        };
        panelHeader.Controls.Add(lblUser);

        cboUsers = new ComboBox
        {
            Font = new Font("Segoe UI", 10),
            Location = new Point(90, 47),
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboUsers.SelectedIndexChanged += (s, e) => LoadUserReport();
        panelHeader.Controls.Add(cboUsers);

        var btnRefresh = new Button
        {
            Text = "🔄 Làm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(310, 45),
            Size = new Size(100, 30),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += (s, e) => LoadUserReport();
        panelHeader.Controls.Add(btnRefresh);

        var btnExport = new Button
        {
            Text = "📄 Xuất",
            Font = new Font("Segoe UI", 10),
            Location = new Point(420, 45),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnExport.FlatAppearance.BorderSize = 0;
        btnExport.Click += BtnExport_Click;
        panelHeader.Controls.Add(btnExport);

        // TabControl cho các bảng
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };

        // Tab 1: Thông tin cơ bản
        var tabInfo = new TabPage("👤 Thông tin cơ bản");
        tabControl.TabPages.Add(tabInfo);
        dgvUserInfo = CreateDataGridView();
        tabInfo.Controls.Add(dgvUserInfo);

        // Tab 2: Roles
        var tabRoles = new TabPage("🎭 Roles");
        tabControl.TabPages.Add(tabRoles);
        dgvRoles = CreateDataGridView();
        tabRoles.Controls.Add(dgvRoles);

        // Tab 3: Privileges
        var tabPrivileges = new TabPage("🔑 Privileges");
        tabControl.TabPages.Add(tabPrivileges);
        dgvPrivileges = CreateDataGridView();
        tabPrivileges.Controls.Add(dgvPrivileges);

        // Tab 4: Quotas
        var tabQuotas = new TabPage("💾 Quotas");
        tabControl.TabPages.Add(tabQuotas);
        dgvQuotas = CreateDataGridView();
        tabQuotas.Controls.Add(dgvQuotas);

        // Thêm controls theo thứ tự: Fill trước, Top sau
        this.Controls.Add(tabControl);
        this.Controls.Add(panelHeader);
    }

    private DataGridView CreateDataGridView()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            ColumnHeadersVisible = true,
            EnableHeadersVisualStyles = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 10)
        };

        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgv.ColumnHeadersHeight = 35;
        dgv.RowTemplate.Height = 30;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

        return dgv;
    }

    private void LoadUsers()
    {
        try
        {
            var users = _userService.GetAllUsers();
            cboUsers.Items.Clear();
            foreach (DataRow row in users.Rows)
            {
                cboUsers.Items.Add(row["USERNAME"].ToString()!);
            }

            if (cboUsers.Items.Count > 0)
                cboUsers.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải danh sách Users: {ex.Message}");
        }
    }

    private void LoadUserReport()
    {
        var username = cboUsers.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(username)) return;

        try
        {
            // Load thông tin cơ bản
            var userInfo = _userService.GetUserDetails(username);
            if (userInfo != null)
            {
                var infoTable = new DataTable();
                infoTable.Columns.Add("Thuộc tính", typeof(string));
                infoTable.Columns.Add("Giá trị", typeof(string));

                infoTable.Rows.Add("Username", userInfo.Username);
                infoTable.Rows.Add("Trạng thái", userInfo.AccountStatus);
                infoTable.Rows.Add("Ngày tạo", userInfo.CreatedDate?.ToString("dd/MM/yyyy HH:mm"));
                infoTable.Rows.Add("Ngày khóa", userInfo.LockDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                infoTable.Rows.Add("Default Tablespace", userInfo.DefaultTablespace);
                infoTable.Rows.Add("Temp Tablespace", userInfo.TemporaryTablespace);
                infoTable.Rows.Add("Profile", userInfo.Profile);

                if (userInfo.UserInfo != null)
                {
                    infoTable.Rows.Add("Họ tên", userInfo.UserInfo.HoTen);
                    infoTable.Rows.Add("Email", userInfo.UserInfo.Email);
                    infoTable.Rows.Add("Số điện thoại", userInfo.UserInfo.SoDienThoai);
                    infoTable.Rows.Add("Phòng ban", userInfo.UserInfo.PhongBan);
                    infoTable.Rows.Add("Chức vụ", userInfo.UserInfo.ChucVu);
                }

                dgvUserInfo.DataSource = infoTable;
            }

            // Load Roles
            var roles = _userService.GetUserRoles(username);
            dgvRoles.DataSource = roles;
            if (dgvRoles.Columns.Count > 0)
            {
                dgvRoles.Columns["GRANTED_ROLE"].HeaderText = "Role";
                dgvRoles.Columns["ADMIN_OPTION"].HeaderText = "Admin Option";
                dgvRoles.Columns["DEFAULT_ROLE"].HeaderText = "Default Role";
            }

            // Load Privileges
            var privileges = _userService.GetUserPrivileges(username);
            dgvPrivileges.DataSource = privileges;
            if (dgvPrivileges.Columns.Count > 0)
            {
                dgvPrivileges.Columns["PRIVILEGE_NAME"].HeaderText = "Quyền";
                dgvPrivileges.Columns["PRIVILEGE_TYPE"].HeaderText = "Loại";
                dgvPrivileges.Columns["SOURCE"].HeaderText = "Nguồn";
                dgvPrivileges.Columns["SOURCE_ROLE"].HeaderText = "Từ Role";
                dgvPrivileges.Columns["ADMIN_OPTION"].HeaderText = "Admin Option";
            }

            // Load Quotas
            var quotas = _userService.GetUserQuotas(username);
            dgvQuotas.DataSource = quotas;
            if (dgvQuotas.Columns.Count > 0)
            {
                dgvQuotas.Columns["TABLESPACE_NAME"].HeaderText = "Tablespace";
                dgvQuotas.Columns["USED_BYTES"].HeaderText = "Đã dùng (bytes)";
                dgvQuotas.Columns["MAX_BYTES"].HeaderText = "Quota tối đa";
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải báo cáo: {ex.Message}");
        }
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        var username = cboUsers.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(username))
        {
            MessageHelper.ShowWarning("Vui lòng chọn User");
            return;
        }

        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = $"UserReport_{username}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using var writer = new StreamWriter(saveDialog.FileName);
                writer.WriteLine($"=== BÁO CÁO USER: {username} ===");
                writer.WriteLine($"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();

                // Thông tin cơ bản
                writer.WriteLine("--- THÔNG TIN CƠ BẢN ---");
                foreach (DataGridViewRow row in dgvUserInfo.Rows)
                {
                    writer.WriteLine($"{row.Cells[0].Value}: {row.Cells[1].Value}");
                }
                writer.WriteLine();

                // Roles
                writer.WriteLine("--- ROLES ---");
                foreach (DataGridViewRow row in dgvRoles.Rows)
                {
                    if (row.Cells["GRANTED_ROLE"].Value != null)
                        writer.WriteLine($"- {row.Cells["GRANTED_ROLE"].Value}");
                }
                writer.WriteLine();

                // Privileges
                writer.WriteLine("--- PRIVILEGES ---");
                foreach (DataGridViewRow row in dgvPrivileges.Rows)
                {
                    if (row.Cells["PRIVILEGE_NAME"].Value != null)
                        writer.WriteLine($"- {row.Cells["PRIVILEGE_NAME"].Value} ({row.Cells["SOURCE"].Value})");
                }

                MessageHelper.ShowSuccess($"Đã xuất báo cáo: {saveDialog.FileName}");
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi xuất báo cáo: {ex.Message}");
        }
    }
}
