using UserManager.BLL.Services;
using UserManager.GUI.UserControls;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form chính của ứng dụng
/// </summary>
public partial class MainForm : Form
{
    private readonly AuthService _authService;

    public MainForm()
    {
        InitializeComponent();
        _authService = new AuthService();
        SetupForm();
        SetupMenu();
        SetupStatusBar();
    }

    private void SetupForm()
    {
        this.Text = $"Quản Lý Người Dùng Oracle - [{AuthService.CurrentSession?.Username}]";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(1200, 700);
        this.MinimumSize = new Size(1000, 600);
        this.BackColor = Color.FromArgb(240, 240, 240);

        // Main Panel để chứa nội dung
        panelMain = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10)
        };
        this.Controls.Add(panelMain);
    }

    private Panel panelMain = null!;
    private MenuStrip menuStrip = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblStatus = null!;
    private ToolStripStatusLabel lblUser = null!;
    private ToolStripStatusLabel lblTime = null!;

    private void SetupMenu()
    {
        menuStrip = new MenuStrip
        {
            Font = new Font("Segoe UI", 10),
            BackColor = Color.FromArgb(0, 102, 204),
            ForeColor = Color.White
        };

        // === Menu Hệ thống ===
        var menuSystem = new ToolStripMenuItem("📁 Hệ thống");
        menuSystem.DropDownItems.Add("🔑 Đổi mật khẩu", null, (s, e) => ShowChangePassword());
        menuSystem.DropDownItems.Add(new ToolStripSeparator());
        menuSystem.DropDownItems.Add("🚪 Đăng xuất", null, (s, e) => Logout());
        menuSystem.DropDownItems.Add("❌ Thoát", null, (s, e) => Application.Exit());
        menuStrip.Items.Add(menuSystem);

        // === Menu Quản lý User ===
        var menuUser = new ToolStripMenuItem("👥 Quản lý User");
        menuUser.DropDownItems.Add("📋 Danh sách User", null, (s, e) => ShowUserList());
        menuUser.DropDownItems.Add("➕ Thêm User mới", null, (s, e) => ShowAddUser());
        menuStrip.Items.Add(menuUser);

        // === Menu Quản lý Role ===
        var menuRole = new ToolStripMenuItem("🎭 Quản lý Role");
        menuRole.DropDownItems.Add("📋 Danh sách Role", null, (s, e) => ShowRoleList());
        menuRole.DropDownItems.Add("➕ Thêm Role mới", null, (s, e) => ShowAddRole());
        menuStrip.Items.Add(menuRole);

        // === Menu Quản lý Profile ===
        var menuProfile = new ToolStripMenuItem("📊 Quản lý Profile");
        menuProfile.DropDownItems.Add("📋 Danh sách Profile", null, (s, e) => ShowProfileList());
        menuProfile.DropDownItems.Add("➕ Thêm Profile mới", null, (s, e) => ShowAddProfile());
        menuStrip.Items.Add(menuProfile);

        // === Menu Quản lý Quyền ===
        var menuPrivilege = new ToolStripMenuItem("🔑 Quản lý Quyền");
        menuPrivilege.DropDownItems.Add("📋 System Privileges", null, (s, e) => ShowSystemPrivileges());
        menuPrivilege.DropDownItems.Add("📋 Object Privileges", null, (s, e) => ShowObjectPrivileges());
        menuPrivilege.DropDownItems.Add(new ToolStripSeparator());
        menuPrivilege.DropDownItems.Add("➕ Grant Quyền", null, (s, e) => ShowGrantPrivilege());
        menuStrip.Items.Add(menuPrivilege);

        // === Menu Báo cáo ===
        var menuReport = new ToolStripMenuItem("📈 Báo cáo");
        menuReport.DropDownItems.Add("📊 Thông tin User đầy đủ", null, (s, e) => ShowUserReport());
        menuStrip.Items.Add(menuReport);

        // === Menu Thông tin bổ sung ===
        var menuInfo = new ToolStripMenuItem("📝 Thông tin bổ sung");
        menuInfo.DropDownItems.Add("📋 Danh sách thông tin cá nhân", null, (s, e) => ShowUserInfoList());
        menuStrip.Items.Add(menuInfo);

        // Style menu items
        foreach (ToolStripMenuItem item in menuStrip.Items)
        {
            item.ForeColor = Color.White;
        }

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);
    }

    private void SetupStatusBar()
    {
        statusStrip = new StatusStrip
        {
            BackColor = Color.FromArgb(45, 45, 48),
            SizingGrip = false
        };

        lblStatus = new ToolStripStatusLabel
        {
            Text = "Sẵn sàng",
            ForeColor = Color.LightGreen,
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblUser = new ToolStripStatusLabel
        {
            Text = $"👤 {AuthService.CurrentSession?.Username ?? "Unknown"}",
            ForeColor = Color.White
        };

        lblTime = new ToolStripStatusLabel
        {
            Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            ForeColor = Color.White
        };

        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblUser, lblTime });
        this.Controls.Add(statusStrip);

        // Timer update time
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        timer.Start();
    }

    private void SetStatus(string message, bool isError = false)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = isError ? Color.Red : Color.LightGreen;
    }

    #region Navigation Methods

    private void LoadControl(UserControl control)
    {
        panelMain.Controls.Clear();
        control.Dock = DockStyle.Fill;
        panelMain.Controls.Add(control);
    }

    private void ShowUserList()
    {
        LoadControl(new UserListControl());
        SetStatus("Đang xem danh sách User");
    }

    private void ShowAddUser()
    {
        using var form = new UserEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            ShowUserList(); // Refresh list
        }
    }

    private void ShowRoleList()
    {
        LoadControl(new RoleListControl());
        SetStatus("Đang xem danh sách Role");
    }

    private void ShowAddRole()
    {
        using var form = new RoleEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            ShowRoleList(); // Refresh list
        }
    }

    private void ShowProfileList()
    {
        LoadControl(new ProfileListControl());
        SetStatus("Đang xem danh sách Profile");
    }

    private void ShowAddProfile()
    {
        using var form = new ProfileEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            ShowProfileList(); // Refresh list
        }
    }

    private void ShowSystemPrivileges()
    {
        LoadControl(new PrivilegeListControl());
        SetStatus("Đang xem System Privileges");
    }

    private void ShowObjectPrivileges()
    {
        LoadControl(new ObjectPrivilegeListControl());
        SetStatus("Đang xem Object Privileges");
    }

    private void ShowGrantPrivilege()
    {
        using var form = new GrantPrivilegeForm();
        form.ShowDialog();
        // Có thể refresh privilege list nếu cần
    }

    private void ShowUserReport()
    {
        LoadControl(new UserReportControl());
        SetStatus("Đang xem báo cáo User");
    }


    private void ShowUserInfoList()
    {
        LoadControl(new UserInfoListControl());
        SetStatus("Đang xem thông tin cá nhân bổ sung");
    }

    private void ShowChangePassword()
    {
        using var form = new ChangePasswordForm();
        form.ShowDialog();
    }

    private void Logout()
    {
        if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _authService.Logout();
            this.DialogResult = DialogResult.Retry; // Signal to restart login
            this.Close();
        }
    }

    #endregion

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // Load User List by default
        if (AuthService.IsAdmin)
        {
            ShowUserList();
        }
        else
        {
            // User thường chỉ xem được thông tin của mình
            ShowUserInfoList();
        }
    }
}
