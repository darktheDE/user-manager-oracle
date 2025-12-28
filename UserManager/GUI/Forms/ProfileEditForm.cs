using UserManager.BLL.Services;
using UserManager.Models;
using UserManager.Common.Helpers;
using UserManager.Common.Constants;
using UserManager.GUI.Core;

namespace UserManager.GUI.Forms;

/// <summary>
/// Form thêm/sửa Profile
/// </summary>
public partial class ProfileEditForm : Form
{
    private readonly ProfileService _profileService;
    private readonly string? _editProfileName;
    private readonly bool _isEditMode;

    private TextBox txtProfileName = null!;
    private ComboBox cboSessionsPerUser = null!;
    private ComboBox cboConnectTime = null!;
    private ComboBox cboIdleTime = null!;

    public ProfileEditForm(string? editProfileName = null)
    {
        InitializeComponent();
        _profileService = new ProfileService();
        _editProfileName = editProfileName;
        _isEditMode = !string.IsNullOrEmpty(editProfileName);
        
        SetupForm();
        
        if (_isEditMode)
        {
            LoadProfileData();
        }
    }

    private void SetupForm()
    {
        this.Text = _isEditMode ? $"Sửa Profile: {_editProfileName}" : "Thêm Profile Mới";
        this.Size = new Size(500, 400);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = AppTheme.ContentBackground;

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.CardBackground,
            Padding = new Padding(30)
        };
        this.Controls.Add(panel);

        int y = 20;
        int labelWidth = 180;
        int inputWidth = 200;

        // Title
        var lblTitle = new Label
        {
            Text = _isEditMode ? "SỬA PROFILE" : "TẠO PROFILE MỚI",
            Font = AppTheme.FontLarge,
            ForeColor = AppTheme.TextTitle,
            AutoSize = true,
            Location = new Point(30, y)
        };
        panel.Controls.Add(lblTitle);
        y += 50;

        // Profile Name
        AddLabel(panel, "Tên Profile:", 30, y);
        txtProfileName = new TextBox
        {
            Font = new Font("Segoe UI", 11),
            Location = new Point(labelWidth, y - 3),
            Width = inputWidth,
            CharacterCasing = CharacterCasing.Upper,
            Enabled = !_isEditMode
        };
        panel.Controls.Add(txtProfileName);
        y += 45;

        // Info label
        var lblInfo = new Label
        {
            Text = "Các giá trị: UNLIMITED, DEFAULT, hoặc số cụ thể",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            Location = new Point(30, y),
            AutoSize = true
        };
        panel.Controls.Add(lblInfo);
        y += 30;

        // SESSIONS_PER_USER
        AddLabel(panel, "SESSIONS_PER_USER:", 30, y);
        cboSessionsPerUser = CreateResourceComboBox(labelWidth, y - 3, inputWidth);
        panel.Controls.Add(cboSessionsPerUser);
        y += 45;

        // CONNECT_TIME
        AddLabel(panel, "CONNECT_TIME (phút):", 30, y);
        cboConnectTime = CreateResourceComboBox(labelWidth, y - 3, inputWidth);
        panel.Controls.Add(cboConnectTime);
        y += 45;

        // IDLE_TIME
        AddLabel(panel, "IDLE_TIME (phút):", 30, y);
        cboIdleTime = CreateResourceComboBox(labelWidth, y - 3, inputWidth);
        panel.Controls.Add(cboIdleTime);
        y += 60;

        // Buttons
        var btnSave = new Button
        {
            Text = "Lưu",
            Font = AppTheme.FontBold,
            Size = new Size(120, 40),
            Location = new Point(120, y),
            BackColor = AppTheme.SuccessButton,
            ForeColor = AppTheme.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;
        panel.Controls.Add(btnSave);

        var btnCancel = new Button
        {
            Text = "Hủy",
            Font = AppTheme.FontRegular,
            Size = new Size(120, 40),
            Location = new Point(250, y),
            BackColor = AppTheme.ContentBackground,
            ForeColor = AppTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = AppTheme.CardBorder;
        btnCancel.Click += (s, e) => this.Close();
        panel.Controls.Add(btnCancel);
    }

    private void AddLabel(Panel panel, string text, int x, int y)
    {
        var label = new Label
        {
            Text = text,
            Font = AppTheme.FontRegular,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(x, y),
            AutoSize = true
        };
        panel.Controls.Add(label);
    }

    private ComboBox CreateResourceComboBox(int x, int y, int width)
    {
        var cbo = new ComboBox
        {
            Font = AppTheme.FontRegular,
            Location = new Point(x, y),
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDown
        };
        cbo.Items.AddRange(new object[] { 
            "DEFAULT", "UNLIMITED", 
            "1", "2", "3", "5", "10", "15", "30", "60", "120", "240", "480" 
        });
        cbo.SelectedIndex = 0;
        return cbo;
    }

    private void LoadProfileData()
    {
        if (string.IsNullOrEmpty(_editProfileName)) return;

        try
        {
            var profile = _profileService.GetProfileDetails(_editProfileName);
            if (profile == null)
            {
                MessageHelper.ShowError("Không tìm thấy Profile");
                this.Close();
                return;
            }

            txtProfileName.Text = profile.ProfileName;
            
            if (!string.IsNullOrEmpty(profile.SessionsPerUser))
                cboSessionsPerUser.Text = profile.SessionsPerUser;
            
            if (!string.IsNullOrEmpty(profile.ConnectTime))
                cboConnectTime.Text = profile.ConnectTime;
            
            if (!string.IsNullOrEmpty(profile.IdleTime))
                cboIdleTime.Text = profile.IdleTime;
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            var profileName = txtProfileName.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(profileName))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên Profile");
                txtProfileName.Focus();
                return;
            }

            var profile = new ProfileModel
            {
                ProfileName = profileName,
                SessionsPerUser = cboSessionsPerUser.Text.Trim().ToUpper(),
                ConnectTime = cboConnectTime.Text.Trim().ToUpper(),
                IdleTime = cboIdleTime.Text.Trim().ToUpper()
            };

            if (_isEditMode)
            {
                _profileService.UpdateProfile(profile);
                MessageHelper.ShowSuccess("Cập nhật Profile thành công!");
            }
            else
            {
                _profileService.CreateProfile(profile);
                MessageHelper.ShowSuccess("Tạo Profile thành công!");
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageHelper.ShowError($"Lỗi: {ex.Message}");
        }
    }
}
