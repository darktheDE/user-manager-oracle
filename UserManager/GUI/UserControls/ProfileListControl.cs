using UserManager.BLL.Services;
using UserManager.Common.Helpers;
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
            Text = "📊 DANH SÁCH PROFILE",
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
        btnAdd.Click += (s, e) => AddProfile();
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
        dgvProfiles = new DataGridView
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
        dgvProfiles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
        dgvProfiles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvProfiles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvProfiles.ColumnHeadersHeight = 40;
        dgvProfiles.RowTemplate.Height = 35;
        dgvProfiles.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

        // Context Menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("✏️ Sửa", null, (s, e) => EditProfile());
        contextMenu.Items.Add("📋 Xem Resources", null, (s, e) => ViewProfileResources());
        contextMenu.Items.Add("👥 Xem Users", null, (s, e) => ViewProfileUsers());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("❌ Xóa", null, (s, e) => DeleteProfile());
        dgvProfiles.ContextMenuStrip = contextMenu;

        // Thêm controls theo thứ tự: Fill trước, Top sau
        this.Controls.Add(dgvProfiles);
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
            if (MessageHelper.ShowConfirm($"⚠️ Bạn có chắc muốn XÓA profile '{profileName}'?"))
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
