using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using UserManager.GUI.Forms;
using UserManager.GUI.Core;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị danh sách Users
/// </summary>
public partial class UserListControl : UserControl
{
    private readonly UserService _userService;
    private DataGridView dgvUsers = null!;
    private TextBox txtSearch = null!;

    public UserListControl()
    {
        InitializeComponent();
        _userService = new UserService();
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

        // Title
        var lblTitle = new Label
        {
            Text = "DANH SÁCH NGƯỜI DÙNG",
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

        // Search TextBox - Left aligned
        txtSearch = new TextBox
        {
            PlaceholderText = "Tìm kiếm username...",
            Font = AppTheme.FontRegular,
            Location = new Point(10, 10),
            Size = new Size(280, 30)
        };
        txtSearch.TextChanged += (s, e) => FilterData();
        panelToolbar.Controls.Add(txtSearch);

        // Add Button - Right aligned (sau Refresh)
        var btnAdd = new Button
        {
            Text = "Thêm mới",
            Font = AppTheme.FontRegular,
            Size = new Size(100, 32),
            BackColor = AppTheme.SuccessButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += (s, e) => AddUser();
        panelToolbar.Controls.Add(btnAdd);

        // Refresh Button - Right aligned (trước Add)
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

        // Position buttons on the right
        panelToolbar.Resize += (s, e) =>
        {
            btnAdd.Location = new Point(panelToolbar.Width - btnAdd.Width - 10, 9);
            btnRefresh.Location = new Point(btnAdd.Left - btnRefresh.Width - 10, 9);
        };

        // Card Panel để chứa DataGridView
        var panelCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(1)
        };

        // DataGridView
        dgvUsers = new DataGridView
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

        // Header style
        dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
        dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderText;
        dgvUsers.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBold;
        dgvUsers.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvUsers.ColumnHeadersHeight = 40;

        // Row style
        dgvUsers.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        dgvUsers.DefaultCellStyle.SelectionBackColor = AppTheme.SidebarActive;
        dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
        dgvUsers.RowTemplate.Height = 38;
        dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAlternate;

        // Context Menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Sửa", null, (s, e) => EditUser());
        contextMenu.Items.Add("Khóa/Mở khóa", null, (s, e) => ToggleLock());
        contextMenu.Items.Add("Đổi mật khẩu", null, (s, e) => ChangePassword());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Xóa", null, (s, e) => DeleteUser());
        dgvUsers.ContextMenuStrip = contextMenu;

        dgvUsers.CellDoubleClick += (s, e) => EditUser();

        panelCard.Controls.Add(dgvUsers);

        // Add controls in correct order for docking
        this.Controls.Add(panelCard);      // Fill
        this.Controls.Add(panelToolbar);   // Top
        this.Controls.Add(panelHeader);    // Top
    }

    private DataTable? _originalData;

    private void LoadData()
    {
        try
        {
            _originalData = _userService.GetAllUsers();
            dgvUsers.DataSource = _originalData;
            FormatColumns();
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private void FormatColumns()
    {
        if (dgvUsers.Columns.Count == 0) return;

        var columnHeaders = new Dictionary<string, string>
        {
            { "USERNAME", "Tên đăng nhập" },
            { "ACCOUNT_STATUS", "Trạng thái" },
            { "LOCK_DATE", "Ngày khóa" },
            { "CREATED_DATE", "Ngày tạo" },
            { "DEFAULT_TABLESPACE", "Tablespace" },
            { "TEMPORARY_TABLESPACE", "Temp" },
            { "PROFILE", "Profile" }
        };

        foreach (var kvp in columnHeaders)
        {
            if (dgvUsers.Columns.Contains(kvp.Key))
            {
                dgvUsers.Columns[kvp.Key].HeaderText = kvp.Value;
            }
        }

        if (dgvUsers.Columns.Contains("USERNAME"))
            dgvUsers.Columns["USERNAME"].FillWeight = 15;
        if (dgvUsers.Columns.Contains("ACCOUNT_STATUS"))
            dgvUsers.Columns["ACCOUNT_STATUS"].FillWeight = 10;
        if (dgvUsers.Columns.Contains("LOCK_DATE"))
            dgvUsers.Columns["LOCK_DATE"].FillWeight = 15;
        if (dgvUsers.Columns.Contains("CREATED_DATE"))
            dgvUsers.Columns["CREATED_DATE"].FillWeight = 15;
        if (dgvUsers.Columns.Contains("DEFAULT_TABLESPACE"))
            dgvUsers.Columns["DEFAULT_TABLESPACE"].FillWeight = 12;
        if (dgvUsers.Columns.Contains("TEMPORARY_TABLESPACE"))
            dgvUsers.Columns["TEMPORARY_TABLESPACE"].FillWeight = 8;
        if (dgvUsers.Columns.Contains("PROFILE"))
            dgvUsers.Columns["PROFILE"].FillWeight = 10;
    }

    private void FilterData()
    {
        if (_originalData == null) return;

        var searchText = txtSearch.Text.Trim().ToUpper();
        if (string.IsNullOrEmpty(searchText))
        {
            dgvUsers.DataSource = _originalData;
        }
        else
        {
            var filteredRows = _originalData.AsEnumerable()
                .Where(r => r["USERNAME"].ToString()!.ToUpper().Contains(searchText));
            
            if (filteredRows.Any())
                dgvUsers.DataSource = filteredRows.CopyToDataTable();
            else
                dgvUsers.DataSource = _originalData.Clone();
        }
    }

    private string? GetSelectedUsername()
    {
        if (dgvUsers.SelectedRows.Count == 0)
        {
            MessageHelper.ShowWarning("Vui lòng chọn một User");
            return null;
        }
        return dgvUsers.SelectedRows[0].Cells["USERNAME"].Value?.ToString();
    }

    private void AddUser()
    {
        using var form = new UserEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void EditUser()
    {
        var username = GetSelectedUsername();
        if (username == null) return;

        using var form = new UserEditForm(username);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void ToggleLock()
    {
        var username = GetSelectedUsername();
        if (username == null) return;

        try
        {
            var status = dgvUsers.SelectedRows[0].Cells["ACCOUNT_STATUS"].Value?.ToString();
            if (status?.Contains("LOCK") == true)
            {
                if (MessageHelper.ShowConfirm($"Bạn có chắc muốn MỞ KHÓA user '{username}'?"))
                {
                    _userService.UnlockUser(username);
                    MessageHelper.ShowSuccess($"Đã mở khóa user '{username}'");
                    LoadData();
                }
            }
            else
            {
                if (MessageHelper.ShowConfirm($"Bạn có chắc muốn KHÓA user '{username}'?"))
                {
                    _userService.LockUser(username);
                    MessageHelper.ShowSuccess($"Đã khóa user '{username}'");
                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void ChangePassword()
    {
        var username = GetSelectedUsername();
        if (username == null) return;

        using var form = new ChangePasswordForm(username);
        form.ShowDialog();
    }

    private void DeleteUser()
    {
        var username = GetSelectedUsername();
        if (username == null) return;

        try
        {
            if (MessageHelper.ShowConfirm($"Bạn có chắc muốn XÓA user '{username}'?\n\nĐây là hành động không thể hoàn tác!"))
            {
                _userService.DeleteUser(username);
                MessageHelper.ShowSuccess($"Đã xóa user '{username}'");
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi xóa user: {ex.Message}");
        }
    }
}
