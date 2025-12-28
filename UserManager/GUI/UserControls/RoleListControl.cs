using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị danh sách Roles
/// </summary>
public partial class RoleListControl : UserControl
{
    private readonly RoleService _roleService;
    private DataGridView dgvRoles = null!;

    public RoleListControl()
    {
        InitializeComponent();
        _roleService = new RoleService();
        SetupUI();
        LoadData();
    }

    private void SetupUI()
    {
        this.BackColor = Color.White;
        this.Padding = new Padding(10);

        // Header Panel
        var panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.White
        };

        // Title
        var lblTitle = new Label
        {
            Text = "🎭 DANH SÁCH ROLE",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(10, 15)
        };
        panelHeader.Controls.Add(lblTitle);

        // Toolbar Panel
        var panelToolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(248, 248, 248)
        };

        // Add Button
        var btnAdd = new Button
        {
            Text = "➕ Thêm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(10, 8),
            Size = new Size(100, 32),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += (s, e) => AddRole();
        panelToolbar.Controls.Add(btnAdd);

        // Refresh Button
        var btnRefresh = new Button
        {
            Text = "🔄 Làm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(120, 8),
            Size = new Size(100, 32),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += (s, e) => LoadData();
        panelToolbar.Controls.Add(btnRefresh);

        // DataGridView
        dgvRoles = new DataGridView
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

        // Header style
        dgvRoles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
        dgvRoles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvRoles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvRoles.ColumnHeadersHeight = 40;
        dgvRoles.RowTemplate.Height = 35;
        dgvRoles.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

        // Context Menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("✏️ Sửa Password", null, (s, e) => EditRolePassword());
        contextMenu.Items.Add("📋 Xem Privileges", null, (s, e) => ViewRolePrivileges());
        contextMenu.Items.Add("📋 Xem Grantees", null, (s, e) => ViewRoleGrantees());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("❌ Xóa", null, (s, e) => DeleteRole());
        dgvRoles.ContextMenuStrip = contextMenu;

        // Thêm controls theo thứ tự: Fill trước, Top sau
        this.Controls.Add(dgvRoles);
        this.Controls.Add(panelToolbar);
        this.Controls.Add(panelHeader);
    }

    private void LoadData()
    {
        try
        {
            var data = _roleService.GetAllRoles();
            dgvRoles.DataSource = data;

            if (dgvRoles.Columns.Count > 0)
            {
                dgvRoles.Columns["ROLE"].HeaderText = "Tên Role";
                dgvRoles.Columns["PASSWORD_REQUIRED"].HeaderText = "Yêu cầu Password";
                dgvRoles.Columns["AUTHENTICATION_TYPE"].HeaderText = "Loại xác thực";
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private string? GetSelectedRoleName()
    {
        if (dgvRoles.SelectedRows.Count == 0)
        {
            MessageHelper.ShowWarning("Vui lòng chọn một Role");
            return null;
        }
        return dgvRoles.SelectedRows[0].Cells["ROLE"].Value?.ToString();
    }

    private void AddRole()
    {
        using var form = new Forms.RoleEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void EditRolePassword()
    {
        var roleName = GetSelectedRoleName();
        if (roleName == null) return;
        
        using var form = new Forms.RoleEditForm(roleName);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void ViewRolePrivileges()
    {
        var roleName = GetSelectedRoleName();
        if (roleName == null) return;

        try
        {
            var sysPrivs = _roleService.GetRoleSystemPrivileges(roleName);
            var objPrivs = _roleService.GetRoleObjectPrivileges(roleName);

            var message = $"=== Privileges của Role '{roleName}' ===\n\n";
            message += $"System Privileges: {sysPrivs.Rows.Count}\n";
            foreach (DataRow row in sysPrivs.Rows)
            {
                message += $"  - {row["PRIVILEGE"]}\n";
            }

            message += $"\nObject Privileges: {objPrivs.Rows.Count}\n";
            foreach (DataRow row in objPrivs.Rows)
            {
                message += $"  - {row["PRIVILEGE"]} ON {row["OWNER"]}.{row["TABLE_NAME"]}\n";
            }

            MessageBox.Show(message, "Privileges của Role", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void ViewRoleGrantees()
    {
        var roleName = GetSelectedRoleName();
        if (roleName == null) return;

        try
        {
            var grantees = _roleService.GetRoleGrantees(roleName);
            
            var message = $"=== Users/Roles được gán '{roleName}' ===\n\n";
            foreach (DataRow row in grantees.Rows)
            {
                message += $"  - {row["GRANTEE"]}";
                if (row["ADMIN_OPTION"].ToString() == "YES")
                    message += " (WITH ADMIN OPTION)";
                message += "\n";
            }

            if (grantees.Rows.Count == 0)
                message += "Chưa có User/Role nào được gán Role này.";

            MessageBox.Show(message, "Grantees của Role", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void DeleteRole()
    {
        var roleName = GetSelectedRoleName();
        if (roleName == null) return;

        try
        {
            if (MessageHelper.ShowConfirm($"⚠️ Bạn có chắc muốn XÓA role '{roleName}'?"))
            {
                _roleService.DeleteRole(roleName);
                MessageHelper.ShowSuccess($"Đã xóa role '{roleName}'");
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi xóa role: {ex.Message}");
        }
    }
}
