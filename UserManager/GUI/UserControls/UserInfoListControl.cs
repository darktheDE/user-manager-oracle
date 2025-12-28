using UserManager.BLL.Services;
using UserManager.DAL.Repositories;
using UserManager.Common.Helpers;
using System.Data;

namespace UserManager.GUI.UserControls;

/// <summary>
/// UserControl hiển thị danh sách thông tin cá nhân bổ sung (USER_INFO)
/// </summary>
public partial class UserInfoListControl : UserControl
{
    private readonly UserInfoRepository _userInfoRepo;
    private DataGridView dgvUserInfo = null!;
    private TextBox txtSearch = null!;

    public UserInfoListControl()
    {
        InitializeComponent();
        _userInfoRepo = new UserInfoRepository();
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
            Text = "📝 THÔNG TIN CÁ NHÂN BỔ SUNG",
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
            PlaceholderText = "🔍 Tìm kiếm theo tên hoặc username...",
            Font = new Font("Segoe UI", 10),
            Location = new Point(10, 10),
            Size = new Size(250, 30)
        };
        txtSearch.TextChanged += (s, e) => FilterData();
        panelToolbar.Controls.Add(txtSearch);

        // Add Button
        var btnAdd = new Button
        {
            Text = "➕ Thêm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(280, 8),
            Size = new Size(100, 32),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += (s, e) => AddUserInfo();
        panelToolbar.Controls.Add(btnAdd);

        // Refresh Button
        var btnRefresh = new Button
        {
            Text = "🔄 Làm mới",
            Font = new Font("Segoe UI", 10),
            Location = new Point(390, 8),
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
        dgvUserInfo = new DataGridView
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
        dgvUserInfo.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
        dgvUserInfo.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvUserInfo.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvUserInfo.ColumnHeadersHeight = 40;
        dgvUserInfo.RowTemplate.Height = 35;
        dgvUserInfo.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

        // Context Menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("✏️ Sửa", null, (s, e) => EditUserInfo());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("❌ Xóa", null, (s, e) => DeleteUserInfo());
        dgvUserInfo.ContextMenuStrip = contextMenu;

        dgvUserInfo.CellDoubleClick += (s, e) => EditUserInfo();

        // Thêm controls theo thứ tự: Fill trước, Top sau
        this.Controls.Add(dgvUserInfo);
        this.Controls.Add(panelToolbar);
        this.Controls.Add(panelHeader);
    }

    private DataTable? _originalData;

    private void LoadData()
    {
        try
        {
            _originalData = _userInfoRepo.GetAll();
            dgvUserInfo.DataSource = _originalData;

            if (dgvUserInfo.Columns.Count > 0)
            {
                // Ẩn cột ID
                if (dgvUserInfo.Columns.Contains("USER_INFO_ID"))
                    dgvUserInfo.Columns["USER_INFO_ID"].Visible = false;
                if (dgvUserInfo.Columns.Contains("IS_ACTIVE"))
                    dgvUserInfo.Columns["IS_ACTIVE"].Visible = false;

                // Đổi tên hiển thị
                if (dgvUserInfo.Columns.Contains("USERNAME"))
                    dgvUserInfo.Columns["USERNAME"].HeaderText = "Username";
                if (dgvUserInfo.Columns.Contains("HO_TEN"))
                    dgvUserInfo.Columns["HO_TEN"].HeaderText = "Họ tên";
                if (dgvUserInfo.Columns.Contains("NGAY_SINH"))
                    dgvUserInfo.Columns["NGAY_SINH"].HeaderText = "Ngày sinh";
                if (dgvUserInfo.Columns.Contains("GIOI_TINH"))
                    dgvUserInfo.Columns["GIOI_TINH"].HeaderText = "Giới tính";
                if (dgvUserInfo.Columns.Contains("DIA_CHI"))
                    dgvUserInfo.Columns["DIA_CHI"].HeaderText = "Địa chỉ";
                if (dgvUserInfo.Columns.Contains("SO_DIEN_THOAI"))
                    dgvUserInfo.Columns["SO_DIEN_THOAI"].HeaderText = "Số điện thoại";
                if (dgvUserInfo.Columns.Contains("EMAIL"))
                    dgvUserInfo.Columns["EMAIL"].HeaderText = "Email";
                if (dgvUserInfo.Columns.Contains("CHUC_VU"))
                    dgvUserInfo.Columns["CHUC_VU"].HeaderText = "Chức vụ";
                if (dgvUserInfo.Columns.Contains("PHONG_BAN"))
                    dgvUserInfo.Columns["PHONG_BAN"].HeaderText = "Phòng ban";
                if (dgvUserInfo.Columns.Contains("MA_NHAN_VIEN"))
                    dgvUserInfo.Columns["MA_NHAN_VIEN"].HeaderText = "Mã NV";
                if (dgvUserInfo.Columns.Contains("CREATED_DATE"))
                    dgvUserInfo.Columns["CREATED_DATE"].HeaderText = "Ngày tạo";
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
            dgvUserInfo.DataSource = _originalData;
        }
        else
        {
            var filteredRows = _originalData.AsEnumerable()
                .Where(r => 
                    r["USERNAME"].ToString()!.ToUpper().Contains(searchText) ||
                    r["HO_TEN"].ToString()!.ToUpper().Contains(searchText));
            
            if (filteredRows.Any())
                dgvUserInfo.DataSource = filteredRows.CopyToDataTable();
            else
                dgvUserInfo.DataSource = _originalData.Clone();
        }
    }

    private string? GetSelectedUsername()
    {
        if (dgvUserInfo.SelectedRows.Count == 0)
        {
            MessageHelper.ShowWarning("Vui lòng chọn một bản ghi");
            return null;
        }
        return dgvUserInfo.SelectedRows[0].Cells["USERNAME"].Value?.ToString();
    }

    private void AddUserInfo()
    {
        MessageHelper.ShowInfo("Chức năng Thêm thông tin cá nhân sẽ được triển khai");
    }

    private void EditUserInfo()
    {
        var username = GetSelectedUsername();
        if (username == null) return;
        MessageHelper.ShowInfo($"Chức năng Sửa thông tin của '{username}' sẽ được triển khai");
    }

    private void DeleteUserInfo()
    {
        var username = GetSelectedUsername();
        if (username == null) return;

        try
        {
            if (MessageHelper.ShowConfirm($"⚠️ Bạn có chắc muốn XÓA thông tin cá nhân của '{username}'?"))
            {
                _userInfoRepo.SoftDelete(username);
                MessageHelper.ShowSuccess($"Đã xóa thông tin của '{username}'");
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }
}
