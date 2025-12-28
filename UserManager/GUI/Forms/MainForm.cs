using UserManager.BLL.Services;
using UserManager.GUI.UserControls;
using UserManager.GUI.Core;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form chính của ứng dụng với sidebar navigation
/// </summary>
public partial class MainForm : Form
{
    private readonly AuthService _authService;
    
    // Layout panels
    private Panel panelSidebar = null!;
    private Panel panelHeader = null!;
    private Panel panelContent = null!;
    private StatusStrip statusStrip = null!;
    
    // Status bar labels
    private ToolStripStatusLabel lblStatus = null!;
    private ToolStripStatusLabel lblUser = null!;
    private ToolStripStatusLabel lblTime = null!;
    
    // Menu buttons (để quản lý active state)
    private readonly List<Panel> _menuItems = new();
    private Panel? _activeMenuItem;

    public MainForm()
    {
        InitializeComponent();
        _authService = new AuthService();
        SetupForm();
        SetupLayout();
        SetupStatusBar();
    }

    private void SetupForm()
    {
        this.Text = $"Quản Lý Người Dùng Oracle - [{AuthService.CurrentSession?.Username}]";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(1280, 750);
        this.MinimumSize = new Size(1000, 600);
        this.BackColor = AppTheme.ContentBackground;
    }

    private void SetupLayout()
    {
        // ===== STATUS BAR (phải thêm trước) =====
        statusStrip = new StatusStrip
        {
            BackColor = AppTheme.StatusBarBackground,
            SizingGrip = false,
            Dock = DockStyle.Bottom
        };
        this.Controls.Add(statusStrip);

        // ===== SIDEBAR =====
        panelSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = AppTheme.SidebarWidth,
            BackColor = AppTheme.SidebarBackground
        };
        
        SetupSidebar();
        this.Controls.Add(panelSidebar);

        // ===== HEADER =====
        panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = AppTheme.HeaderHeight,
            BackColor = AppTheme.HeaderBackground,
            Padding = new Padding(15, 0, 15, 0)
        };
        
        SetupHeader();
        this.Controls.Add(panelHeader);

        // ===== CONTENT AREA =====
        panelContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.ContentBackground,
            Padding = new Padding(15)
        };
        this.Controls.Add(panelContent);

        // Đảm bảo thứ tự Z-order đúng
        panelContent.BringToFront();
    }

    private void SetupSidebar()
    {
        int yPos = 0;

        // ===== LOGO/TITLE SECTION =====
        var panelLogo = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(24, 24, 37) // Match header - darker than sidebar
        };

        var lblLogo = new Label
        {
            Text = "🗄️ UserManager",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = AppTheme.SidebarText,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };
        panelLogo.Controls.Add(lblLogo);

        // ===== USER INFO =====
        var panelUserInfo = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = AppTheme.SidebarBackground,
            Padding = new Padding(15, 5, 15, 5)
        };

        var lblCurrentUser = new Label
        {
            Text = $"👤 {AuthService.CurrentSession?.Username ?? "User"}",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.SidebarText,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        panelUserInfo.Controls.Add(lblCurrentUser);

        // ===== SEPARATOR =====
        var separator1 = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = AppTheme.SidebarHover
        };

        // ===== MENU CONTAINER =====
        var panelMenu = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SidebarBackground,
            AutoScroll = true
        };

        // Menu items
        yPos = 10;
        
        // Chỉ hiển thị menu đầy đủ cho Admin
        if (AuthService.IsAdmin)
        {
            yPos = AddMenuItem(panelMenu, "👥", "Quản lý User", ShowUserList, yPos);
            yPos = AddMenuItem(panelMenu, "🎭", "Quản lý Role", ShowRoleList, yPos);
            yPos = AddMenuItem(panelMenu, "📊", "Quản lý Profile", ShowProfileList, yPos);
            yPos = AddMenuItem(panelMenu, "🔑", "Quyền hệ thống", ShowSystemPrivileges, yPos);
            yPos = AddMenuItem(panelMenu, "📦", "Quyền đối tượng", ShowObjectPrivileges, yPos);
            yPos = AddMenuItem(panelMenu, "➕", "Cấp quyền", ShowGrantPrivilege, yPos);
            yPos = AddMenuItem(panelMenu, "📈", "Báo cáo", ShowUserReport, yPos);
        }
        
        yPos = AddMenuItem(panelMenu, "📝", "Thông tin cá nhân", ShowUserInfoList, yPos);
        
        // Separator
        yPos += 10;
        var menuSeparator = new Panel
        {
            Location = new Point(15, yPos),
            Size = new Size(AppTheme.SidebarWidth - 30, 1),
            BackColor = AppTheme.SidebarHover
        };
        panelMenu.Controls.Add(menuSeparator);
        yPos += 15;

        // System menu items
        yPos = AddMenuItem(panelMenu, "🔐", "Đổi mật khẩu", ShowChangePassword, yPos);
        yPos = AddMenuItem(panelMenu, "🚪", "Đăng xuất", Logout, yPos);

        // QUAN TRỌNG: Thêm controls theo thứ tự đúng cho Docking
        // 1. Thêm Fill panel đầu tiên
        panelSidebar.Controls.Add(panelMenu);
        // 2. Thêm Top panels theo thứ tự ngược lại (cuối cùng thêm = ở trên cùng)
        panelSidebar.Controls.Add(separator1);
        panelSidebar.Controls.Add(panelUserInfo);
        panelSidebar.Controls.Add(panelLogo);
    }

    private int AddMenuItem(Panel container, string icon, string text, Action onClick, int yPos)
    {
        var panel = new Panel
        {
            Location = new Point(0, yPos),
            Size = new Size(AppTheme.SidebarWidth, AppTheme.MenuItemHeight),
            BackColor = AppTheme.SidebarBackground,
            Cursor = Cursors.Hand
        };

        var lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI", 12),
            ForeColor = AppTheme.SidebarText,
            Location = new Point(15, 0),
            Size = new Size(30, AppTheme.MenuItemHeight),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblText = new Label
        {
            Text = text,
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.SidebarText,
            Location = new Point(50, 0),
            Size = new Size(AppTheme.SidebarWidth - 60, AppTheme.MenuItemHeight),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Hover effects
        void OnHover(object? s, EventArgs e)
        {
            if (panel != _activeMenuItem)
                panel.BackColor = AppTheme.SidebarHover;
        }

        void OnLeave(object? s, EventArgs e)
        {
            if (panel != _activeMenuItem)
                panel.BackColor = AppTheme.SidebarBackground;
        }

        void OnClick(object? s, EventArgs e)
        {
            SetActiveMenuItem(panel);
            onClick();
        }

        panel.MouseEnter += OnHover;
        panel.MouseLeave += OnLeave;
        panel.Click += OnClick;
        
        lblIcon.MouseEnter += OnHover;
        lblIcon.MouseLeave += OnLeave;
        lblIcon.Click += OnClick;
        
        lblText.MouseEnter += OnHover;
        lblText.MouseLeave += OnLeave;
        lblText.Click += OnClick;

        panel.Controls.Add(lblIcon);
        panel.Controls.Add(lblText);
        container.Controls.Add(panel);
        
        _menuItems.Add(panel);

        return yPos + AppTheme.MenuItemHeight;
    }

    private void SetActiveMenuItem(Panel? menuItem)
    {
        // Reset previous active
        if (_activeMenuItem != null)
        {
            _activeMenuItem.BackColor = AppTheme.SidebarBackground;
        }

        // Set new active
        _activeMenuItem = menuItem;
        if (_activeMenuItem != null)
        {
            _activeMenuItem.BackColor = AppTheme.SidebarActive;
        }
    }

    private void SetupHeader()
    {
        // Title in header
        var lblTitle = new Label
        {
            Text = "Hệ thống Quản lý Người dùng Oracle",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.HeaderText,
            AutoSize = true,
            Location = new Point(10, 15)
        };
        panelHeader.Controls.Add(lblTitle);

        // Time label on the right
        var lblHeaderTime = new Label
        {
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.HeaderText,
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        lblHeaderTime.Location = new Point(panelHeader.Width - 150, 15);
        panelHeader.Controls.Add(lblHeaderTime);

        // Timer to update time
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => lblHeaderTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        timer.Start();
        lblHeaderTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void SetupStatusBar()
    {
        lblStatus = new ToolStripStatusLabel
        {
            Text = "Sẵn sàng",
            ForeColor = AppTheme.StatusBarSuccess,
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblUser = new ToolStripStatusLabel
        {
            Text = $"👤 {AuthService.CurrentSession?.Username ?? "Unknown"}",
            ForeColor = AppTheme.StatusBarText
        };

        lblTime = new ToolStripStatusLabel
        {
            Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            ForeColor = AppTheme.StatusBarText
        };

        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblUser, lblTime });

        // Timer update time
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        timer.Start();
    }

    private void SetStatus(string message, bool isError = false)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = isError ? AppTheme.DangerButton : AppTheme.StatusBarSuccess;
    }

    #region Navigation Methods

    private void LoadControl(UserControl control)
    {
        panelContent.Controls.Clear();
        control.Dock = DockStyle.Fill;
        panelContent.Controls.Add(control);
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
    }

    private void ShowUserReport()
    {
        LoadControl(new UserReportControl());
        SetStatus("Đang xem báo cáo User");
    }

    private void ShowUserInfoList()
    {
        LoadControl(new UserInfoListControl());
        SetStatus("Đang xem thông tin cá nhân");
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
        
        // Set first menu item as active and load default view
        if (AuthService.IsAdmin)
        {
            if (_menuItems.Count > 0)
                SetActiveMenuItem(_menuItems[0]);
            ShowUserList();
        }
        else
        {
            // User thường chỉ xem được thông tin của mình
            var userInfoIndex = _menuItems.FindIndex(p => 
                p.Controls.OfType<Label>().Any(l => l.Text.Contains("Thông tin")));
            if (userInfoIndex >= 0)
                SetActiveMenuItem(_menuItems[userInfoIndex]);
            ShowUserInfoList();
        }
    }
}
