using UserManager.BLL.Services;
using UserManager.DAL.Repositories;
using UserManager.Common.Helpers;
using UserManager.GUI.Core;
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
            Text = "THÔNG TIN CÁ NHÂN BỔ SUNG",
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
            PlaceholderText = "Tìm kiếm theo tên hoặc username...",
            Font = AppTheme.FontRegular,
            Location = new Point(10, 10),
            Size = new Size(280, 30)
        };
        txtSearch.TextChanged += (s, e) => FilterData();
        panelToolbar.Controls.Add(txtSearch);

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
        btnAdd.Click += (s, e) => AddUserInfo();
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
        dgvUserInfo = new DataGridView
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

        dgvUserInfo.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
        dgvUserInfo.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderText;
        dgvUserInfo.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBold;
        dgvUserInfo.ColumnHeadersHeight = 40;
        dgvUserInfo.RowTemplate.Height = 38;
        dgvUserInfo.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAlternate;
        dgvUserInfo.DefaultCellStyle.SelectionBackColor = AppTheme.SidebarActive;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Sửa", null, (s, e) => EditUserInfo());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Xóa", null, (s, e) => DeleteUserInfo());
        dgvUserInfo.ContextMenuStrip = contextMenu;

        dgvUserInfo.CellDoubleClick += (s, e) => EditUserInfo();

        panelCard.Controls.Add(dgvUserInfo);

        this.Controls.Add(panelCard);
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
                if (dgvUserInfo.Columns.Contains("USER_INFO_ID"))
                    dgvUserInfo.Columns["USER_INFO_ID"].Visible = false;
                if (dgvUserInfo.Columns.Contains("IS_ACTIVE"))
                    dgvUserInfo.Columns["IS_ACTIVE"].Visible = false;

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
            if (MessageHelper.ShowConfirm($"Bạn có chắc muốn XÓA thông tin cá nhân của '{username}'?"))
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
