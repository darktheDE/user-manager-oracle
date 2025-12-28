using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using UserManager.GUI.Core;
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
        this.BackColor = AppTheme.ContentBackground;
        this.Padding = new Padding(15);

        // Header Panel
        var panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = AppTheme.ContentBackground
        };

        var lblTitle = new Label
        {
            Text = "DANH SÁCH OBJECT PRIVILEGES",
            Font = AppTheme.FontLarge,
            ForeColor = AppTheme.TextTitle,
            AutoSize = true,
            Location = new Point(5, 12)
        };
        panelHeader.Controls.Add(lblTitle);

        // Toolbar Panel
        var panelToolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(10, 8, 10, 8)
        };

        // Search TextBox
        txtSearch = new TextBox
        {
            PlaceholderText = "Tìm kiếm...",
            Font = AppTheme.FontRegular,
            Location = new Point(10, 10),
            Size = new Size(280, 30)
        };
        txtSearch.TextChanged += (s, e) => FilterData();
        panelToolbar.Controls.Add(txtSearch);

        // Refresh Button
        var btnRefresh = new Button
        {
            Text = "Làm mới",
            Font = AppTheme.FontRegular,
            Size = new Size(100, 32),
            BackColor = AppTheme.PrimaryButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += (s, e) => LoadData();
        panelToolbar.Controls.Add(btnRefresh);

        panelToolbar.Resize += (s, e) =>
        {
            btnRefresh.Location = new Point(panelToolbar.Width - btnRefresh.Width - 10, 9);
        };

        // Card Panel
        var panelCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(1)
        };

        // DataGridView
        dgvPrivileges = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.CardBackground,
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
            Font = AppTheme.FontRegular,
            GridColor = AppTheme.GridBorder
        };

        dgvPrivileges.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
        dgvPrivileges.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderText;
        dgvPrivileges.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBold;
        dgvPrivileges.ColumnHeadersHeight = 40;
        dgvPrivileges.RowTemplate.Height = 38;
        dgvPrivileges.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAlternate;
        dgvPrivileges.DefaultCellStyle.SelectionBackColor = AppTheme.SidebarActive;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Revoke", null, (s, e) => RevokePrivilege());
        dgvPrivileges.ContextMenuStrip = contextMenu;

        panelCard.Controls.Add(dgvPrivileges);

        this.Controls.Add(panelCard);
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
            if (MessageHelper.ShowConfirm($"Thu hồi quyền '{privilege}' trên '{owner}.{tableName}' từ '{grantee}'?"))
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
