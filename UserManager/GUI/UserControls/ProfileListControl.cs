using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using UserManager.GUI.Core;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị danh sách Profiles
/// </summary>
public partial class ProfileListControl : UserControl
{
    private readonly ProfileService _profileService;
    private DataGridView dgvProfiles = null!;

    public ProfileListControl()
    {
        InitializeComponent();
        _profileService = new ProfileService();
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
            Text = "DANH SÁCH PROFILE",
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

        // Add Button
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
        btnAdd.Click += (s, e) => AddProfile();
        panelToolbar.Controls.Add(btnAdd);

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
            btnAdd.Location = new Point(panelToolbar.Width - btnAdd.Width - 10, 9);
            btnRefresh.Location = new Point(btnAdd.Left - btnRefresh.Width - 10, 9);
        };

        // Card Panel
        var panelCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(1)
        };

        // DataGridView
        dgvProfiles = new DataGridView
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

        dgvProfiles.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
        dgvProfiles.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderText;
        dgvProfiles.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBold;
        dgvProfiles.ColumnHeadersHeight = 40;
        dgvProfiles.RowTemplate.Height = 38;
        dgvProfiles.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAlternate;
        dgvProfiles.DefaultCellStyle.SelectionBackColor = AppTheme.SidebarActive;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Sửa", null, (s, e) => EditProfile());
        contextMenu.Items.Add("Xem Resources", null, (s, e) => ViewProfileResources());
        contextMenu.Items.Add("Xem Users", null, (s, e) => ViewProfileUsers());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Xóa", null, (s, e) => DeleteProfile());
        dgvProfiles.ContextMenuStrip = contextMenu;

        panelCard.Controls.Add(dgvProfiles);

        this.Controls.Add(panelCard);
        this.Controls.Add(panelToolbar);
        this.Controls.Add(panelHeader);
    }

    private void LoadData()
    {
        try
        {
            var data = _profileService.GetAllProfiles();
            dgvProfiles.DataSource = data;

            if (dgvProfiles.Columns.Count > 0)
            {
                dgvProfiles.Columns["PROFILE"].HeaderText = "Tên Profile";
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private string? GetSelectedProfileName()
    {
        if (dgvProfiles.SelectedRows.Count == 0)
        {
            MessageHelper.ShowWarning("Vui lòng chọn một Profile");
            return null;
        }
        return dgvProfiles.SelectedRows[0].Cells["PROFILE"].Value?.ToString();
    }

    private void AddProfile()
    {
        using var form = new Forms.ProfileEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void EditProfile()
    {
        var profileName = GetSelectedProfileName();
        if (profileName == null) return;
        
        using var form = new Forms.ProfileEditForm(profileName);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void ViewProfileResources()
    {
        var profileName = GetSelectedProfileName();
        if (profileName == null) return;

        try
        {
            var details = _profileService.GetProfileDetails(profileName);
            if (details == null)
            {
                MessageHelper.ShowWarning("Không tìm thấy thông tin Profile");
                return;
            }

            var message = $"=== Resources của Profile '{profileName}' ===\n\n";
            message += $"SESSIONS_PER_USER: {details.SessionsPerUser ?? "DEFAULT"}\n";
            message += $"CONNECT_TIME: {details.ConnectTime ?? "DEFAULT"}\n";
            message += $"IDLE_TIME: {details.IdleTime ?? "DEFAULT"}\n";

            MessageBox.Show(message, "Profile Resources", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void ViewProfileUsers()
    {
        var profileName = GetSelectedProfileName();
        if (profileName == null) return;

        try
        {
            var users = _profileService.GetProfileUsers(profileName);
            
            var message = $"=== Users sử dụng Profile '{profileName}' ===\n\n";
            foreach (DataRow row in users.Rows)
            {
                message += $"  - {row["USERNAME"]} ({row["ACCOUNT_STATUS"]})\n";
            }

            if (users.Rows.Count == 0)
                message += "Chưa có User nào sử dụng Profile này.";

            MessageBox.Show(message, "Profile Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void DeleteProfile()
    {
        var profileName = GetSelectedProfileName();
        if (profileName == null) return;

        try
        {
            if (MessageHelper.ShowConfirm($"Bạn có chắc muốn XÓA profile '{profileName}'?"))
            {
                _profileService.DeleteProfile(profileName);
                MessageHelper.ShowSuccess($"Đã xóa profile '{profileName}'");
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi xóa profile: {ex.Message}");
        }
    }
}
