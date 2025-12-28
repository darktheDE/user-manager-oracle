using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using UserManager.Common.Constants;
using UserManager.GUI.Core;
using System.Data;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form Grant Privilege
/// </summary>
public partial class GrantPrivilegeForm : Form
{
    private readonly PrivilegeService _privilegeService;
    private readonly UserService _userService;
    private readonly RoleService _roleService;

    private TabControl tabControl = null!;
    private ComboBox cboGrantee = null!;
    private RadioButton rdoUser = null!;
    private RadioButton rdoRole = null!;

    // System Privilege tab
    private CheckedListBox clbSystemPrivileges = null!;
    private CheckBox chkWithAdminOption = null!;

    // Object Privilege tab
    private TextBox txtOwner = null!;
    private TextBox txtObjectName = null!;
    private CheckedListBox clbObjectPrivileges = null!;
    private CheckBox chkWithGrantOption = null!;

    // Role Grant tab
    private CheckedListBox clbRoles = null!;
    private CheckBox chkRoleAdminOption = null!;

    public GrantPrivilegeForm()
    {
        InitializeComponent();
        _privilegeService = new PrivilegeService();
        _userService = new UserService();
        _roleService = new RoleService();
        
        SetupForm();
        LoadData();
    }

    private void SetupForm()
    {
        this.Text = "Grant Quyền";
        this.Size = new Size(600, 550);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = AppTheme.ContentBackground;

        // Header Panel
        var panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(20)
        };

        // Title
        var lblTitle = new Label
        {
            Text = "GRANT QUYỀN",
            Font = AppTheme.FontLarge,
            ForeColor = AppTheme.TextTitle,
            AutoSize = true,
            Location = new Point(20, 10)
        };
        panelHeader.Controls.Add(lblTitle);

        // Grantee type
        rdoUser = new RadioButton
        {
            Text = "User",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(20, 45),
            AutoSize = true,
            Checked = true
        };
        rdoUser.CheckedChanged += (s, e) => LoadGrantees();
        panelHeader.Controls.Add(rdoUser);

        rdoRole = new RadioButton
        {
            Text = "Role",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(100, 45),
            AutoSize = true
        };
        panelHeader.Controls.Add(rdoRole);

        // Grantee ComboBox
        var lblGrantee = new Label
        {
            Text = "Gán cho:",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(180, 48),
            AutoSize = true
        };
        panelHeader.Controls.Add(lblGrantee);

        cboGrantee = new ComboBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(250, 45),
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        panelHeader.Controls.Add(cboGrantee);

        // TabControl
        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = AppTheme.FontRegular
        };

        // Tab 1: System Privileges
        var tabSystem = new TabPage("System Privilege");
        tabControl.TabPages.Add(tabSystem);
        SetupSystemPrivilegeTab(tabSystem);

        // Tab 2: Object Privileges
        var tabObject = new TabPage("Object Privilege");
        tabControl.TabPages.Add(tabObject);
        SetupObjectPrivilegeTab(tabObject);

        // Tab 3: Role Grant
        var tabRole = new TabPage("Grant Role");
        tabControl.TabPages.Add(tabRole);
        SetupRoleGrantTab(tabRole);

        // Button Panel
        var panelButtons = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = AppTheme.CardBackground
        };

        var btnGrant = new Button
        {
            Text = "Grant",
            Font = AppTheme.FontBold,
            Size = new Size(120, 40),
            Location = new Point(200, 10),
            BackColor = AppTheme.SuccessButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnGrant.FlatAppearance.BorderSize = 0;
        btnGrant.Click += BtnGrant_Click;
        panelButtons.Controls.Add(btnGrant);

        var btnClose = new Button
        {
            Text = "Đóng",
            Font = AppTheme.FontRegular,
            Size = new Size(120, 40),
            Location = new Point(330, 10),
            BackColor = AppTheme.ContentBackground,
            ForeColor = AppTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 1;
        btnClose.FlatAppearance.BorderColor = AppTheme.CardBorder;
        btnClose.Click += (s, e) => this.Close();
        panelButtons.Controls.Add(btnClose);

        // Thêm controls theo thứ tự đúng
        this.Controls.Add(tabControl);
        this.Controls.Add(panelButtons);
        this.Controls.Add(panelHeader);
    }

    private void SetupSystemPrivilegeTab(TabPage tab)
    {
        tab.BackColor = AppTheme.CardBackground;
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = AppTheme.CardBackground };
        tab.Controls.Add(panel);

        var lblInfo = new Label
        {
            Text = "Chọn các System Privileges cần grant:",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, 10),
            AutoSize = true
        };
        panel.Controls.Add(lblInfo);

        clbSystemPrivileges = new CheckedListBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(10, 40),
            Size = new Size(520, 250),
            CheckOnClick = true,
            BackColor = AppTheme.CardBackground
        };
        foreach (var priv in SystemPrivileges.All)
        {
            clbSystemPrivileges.Items.Add(priv);
        }
        panel.Controls.Add(clbSystemPrivileges);

        chkWithAdminOption = new CheckBox
        {
            Text = "WITH ADMIN OPTION (cho phép người nhận grant tiếp cho người khác)",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, 300),
            AutoSize = true
        };
        panel.Controls.Add(chkWithAdminOption);
    }

    private void SetupObjectPrivilegeTab(TabPage tab)
    {
        tab.BackColor = AppTheme.CardBackground;
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = AppTheme.CardBackground };
        tab.Controls.Add(panel);

        int y = 10;

        // Owner
        var lblOwner = new Label
        {
            Text = "Owner (Schema):",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, y),
            AutoSize = true
        };
        panel.Controls.Add(lblOwner);

        txtOwner = new TextBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(150, y - 3),
            Width = 200,
            CharacterCasing = CharacterCasing.Upper,
            Text = "SYSTEM"
        };
        panel.Controls.Add(txtOwner);
        y += 40;

        // Object Name
        var lblObject = new Label
        {
            Text = "Tên Table/View:",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, y),
            AutoSize = true
        };
        panel.Controls.Add(lblObject);

        txtObjectName = new TextBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(150, y - 3),
            Width = 200,
            CharacterCasing = CharacterCasing.Upper
        };
        panel.Controls.Add(txtObjectName);
        y += 40;

        // Privileges
        var lblPrivs = new Label
        {
            Text = "Chọn quyền:",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, y),
            AutoSize = true
        };
        panel.Controls.Add(lblPrivs);
        y += 25;

        clbObjectPrivileges = new CheckedListBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(10, y),
            Size = new Size(200, 150),
            CheckOnClick = true,
            BackColor = AppTheme.CardBackground
        };
        foreach (var priv in ObjectPrivileges.OnTable)
        {
            clbObjectPrivileges.Items.Add(priv);
        }
        panel.Controls.Add(clbObjectPrivileges);
        y += 160;

        chkWithGrantOption = new CheckBox
        {
            Text = "WITH GRANT OPTION",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, y),
            AutoSize = true
        };
        panel.Controls.Add(chkWithGrantOption);
    }

    private void SetupRoleGrantTab(TabPage tab)
    {
        tab.BackColor = AppTheme.CardBackground;
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = AppTheme.CardBackground };
        tab.Controls.Add(panel);

        var lblInfo = new Label
        {
            Text = "Chọn các Role cần grant:",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, 10),
            AutoSize = true
        };
        panel.Controls.Add(lblInfo);

        clbRoles = new CheckedListBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(10, 40),
            Size = new Size(520, 250),
            CheckOnClick = true,
            BackColor = AppTheme.CardBackground
        };
        panel.Controls.Add(clbRoles);

        chkRoleAdminOption = new CheckBox
        {
            Text = "WITH ADMIN OPTION",
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, 300),
            AutoSize = true
        };
        panel.Controls.Add(chkRoleAdminOption);
    }

    private void LoadData()
    {
        LoadGrantees();
        LoadRoles();
    }

    private void LoadGrantees()
    {
        cboGrantee.Items.Clear();
        
        try
        {
            if (rdoUser.Checked)
            {
                var users = _userService.GetAllUsers();
                foreach (DataRow row in users.Rows)
                {
                    cboGrantee.Items.Add(row["USERNAME"].ToString()!);
                }
            }
            else
            {
                var roles = _roleService.GetAllRoles();
                foreach (DataRow row in roles.Rows)
                {
                    cboGrantee.Items.Add(row["ROLE"].ToString()!);
                }
            }

            if (cboGrantee.Items.Count > 0)
                cboGrantee.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
        }
    }

    private void LoadRoles()
    {
        clbRoles.Items.Clear();
        
        try
        {
            var roles = _roleService.GetAllRoles();
            foreach (DataRow row in roles.Rows)
            {
                clbRoles.Items.Add(row["ROLE"].ToString()!);
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi tải Roles: {ex.Message}");
        }
    }

    private void BtnGrant_Click(object? sender, EventArgs e)
    {
        try
        {
            var grantee = cboGrantee.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(grantee))
            {
                MessageHelper.ShowWarning("Vui lòng chọn User/Role để grant");
                return;
            }

            int grantedCount = 0;

            switch (tabControl.SelectedIndex)
            {
                case 0: // System Privileges
                    foreach (var item in clbSystemPrivileges.CheckedItems)
                    {
                        _privilegeService.GrantSystemPrivilege(item.ToString()!, grantee, chkWithAdminOption.Checked);
                        grantedCount++;
                    }
                    break;

                case 1: // Object Privileges
                    if (string.IsNullOrWhiteSpace(txtOwner.Text) || string.IsNullOrWhiteSpace(txtObjectName.Text))
                    {
                        MessageHelper.ShowWarning("Vui lòng nhập Owner và tên Object");
                        return;
                    }
                    foreach (var item in clbObjectPrivileges.CheckedItems)
                    {
                        _privilegeService.GrantObjectPrivilege(
                            item.ToString()!, 
                            txtOwner.Text.Trim().ToUpper(), 
                            txtObjectName.Text.Trim().ToUpper(), 
                            grantee, 
                            chkWithGrantOption.Checked
                        );
                        grantedCount++;
                    }
                    break;

                case 2: // Role Grant
                    foreach (var item in clbRoles.CheckedItems)
                    {
                        _privilegeService.GrantRole(item.ToString()!, grantee, chkRoleAdminOption.Checked);
                        grantedCount++;
                    }
                    break;
            }

            if (grantedCount > 0)
            {
                MessageHelper.ShowSuccess($"Đã grant {grantedCount} quyền cho '{grantee}'");
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageHelper.ShowWarning("Vui lòng chọn ít nhất một quyền để grant");
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi grant quyền: {ex.Message}");
        }
    }
}
