using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị danh sách Object Privileges
/// </summary>
public partial class ObjectPrivilegeListControl : UserControl
{
    private readonly PrivilegeService _privilegeService;
    private DataGridView dgvPrivileges = null!;
    private TextBox txtSearch = null!;

    public ObjectPrivilegeListControl()
    {
        InitializeComponent();
        _privilegeService = new PrivilegeService();
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
            Text = "📦 DANH SÁCH OBJECT PRIVILEGES",
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

        // Search TextBox
        txtSearch = new TextBox
        {
            PlaceholderText = "🔍 Tìm kiếm...",
            Font = new Font("Segoe UI", 10),
            Location = new Point(10, 10),
            Size = new Size(250, 30)
        };
        txtSearch.TextChanged += (s, e) => FilterData();
        panelToolbar.Controls.Add(txtSearch);

        // Refresh Button
        var btnRefresh = new Button
        {
            Text = "🔄 Làm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(280, 8),
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
        dgvPrivileges = new DataGridView
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
        dgvPrivileges.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
        dgvPrivileges.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvPrivileges.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvPrivileges.ColumnHeadersHeight = 40;
        dgvPrivileges.RowTemplate.Height = 35;
        dgvPrivileges.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

        // Context Menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("❌ Revoke", null, (s, e) => RevokePrivilege());
        dgvPrivileges.ContextMenuStrip = contextMenu;

        // Thêm controls theo thứ tự: Fill trước, Top sau
        this.Controls.Add(dgvPrivileges);
        this.Controls.Add(panelToolbar);
        this.Controls.Add(panelHeader);
    }

    private DataTable? _originalData;

    private void LoadData()
    {
        try
        {
            _originalData = _privilegeService.GetAllObjectPrivileges();
            dgvPrivileges.DataSource = _originalData;

            if (dgvPrivileges.Columns.Count > 0)
            {
                dgvPrivileges.Columns["GRANTEE"].HeaderText = "Người nhận";
                dgvPrivileges.Columns["PRIVILEGE"].HeaderText = "Quyền";
                dgvPrivileges.Columns["OWNER"].HeaderText = "Owner";
                dgvPrivileges.Columns["TABLE_NAME"].HeaderText = "Table/View";
                dgvPrivileges.Columns["GRANTABLE"].HeaderText = "Grant Option";
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private void FilterData()
    {
        if (_originalData == null) return;

        var searchText = txtSearch.Text.Trim().ToUpper();
        if (string.IsNullOrEmpty(searchText))
        {
            dgvPrivileges.DataSource = _originalData;
        }
        else
        {
            var filteredRows = _originalData.AsEnumerable()
                .Where(r => 
                    r["GRANTEE"].ToString()!.ToUpper().Contains(searchText) ||
                    r["PRIVILEGE"].ToString()!.ToUpper().Contains(searchText) ||
                    r["OWNER"].ToString()!.ToUpper().Contains(searchText) ||
                    r["TABLE_NAME"].ToString()!.ToUpper().Contains(searchText));
            
            if (filteredRows.Any())
                dgvPrivileges.DataSource = filteredRows.CopyToDataTable();
            else
                dgvPrivileges.DataSource = _originalData.Clone();
        }
    }

    private void RevokePrivilege()
    {
        if (dgvPrivileges.SelectedRows.Count == 0)
        {
            MessageHelper.ShowWarning("Vui lòng chọn một privilege để thu hồi");
            return;
        }

        var row = dgvPrivileges.SelectedRows[0];
        var grantee = row.Cells["GRANTEE"].Value?.ToString();
        var privilege = row.Cells["PRIVILEGE"].Value?.ToString();
        var owner = row.Cells["OWNER"].Value?.ToString();
        var tableName = row.Cells["TABLE_NAME"].Value?.ToString();

        if (string.IsNullOrEmpty(grantee) || string.IsNullOrEmpty(privilege))
            return;

        try
        {
            if (MessageHelper.ShowConfirm($"⚠️ Thu hồi quyền '{privilege}' trên '{owner}.{tableName}' từ '{grantee}'?"))
            {
                _privilegeService.RevokeObjectPrivilege(privilege, owner!, tableName!, grantee);
                MessageHelper.ShowSuccess($"Đã thu hồi quyền");
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }
}
