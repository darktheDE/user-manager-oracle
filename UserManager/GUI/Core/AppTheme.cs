namespace UserManager.GUI.Core;

/// <summary>
/// Định nghĩa các màu sắc và style cho toàn bộ ứng dụng
/// Theme: Modern Dark Purple/Violet
/// </summary>
public static class AppTheme
{
    // ============== SIDEBAR ==============
    /// <summary>Màu nền sidebar chính - Dark charcoal</summary>
    public static Color SidebarBackground => Color.FromArgb(30, 30, 46);      // Dark charcoal blue
    
    /// <summary>Màu nền khi hover menu item</summary>
    public static Color SidebarHover => Color.FromArgb(45, 45, 65);           // Lighter hover
    
    /// <summary>Màu nền menu item đang active - Violet</summary>
    public static Color SidebarActive => Color.FromArgb(139, 92, 246);        // Violet/Purple accent
    
    /// <summary>Màu chữ trên sidebar</summary>
    public static Color SidebarText => Color.FromArgb(248, 250, 252);         // Almost white
    
    /// <summary>Màu chữ mờ hơn</summary>
    public static Color SidebarTextMuted => Color.FromArgb(148, 163, 184);    // Slate gray

    // ============== HEADER ==============
    public static Color HeaderBackground => Color.FromArgb(24, 24, 37);       // Darker than sidebar
    public static Color HeaderText => Color.FromArgb(248, 250, 252);

    // ============== CONTENT AREA ==============
    public static Color ContentBackground => Color.FromArgb(241, 245, 249);   // Slate 100
    public static Color CardBackground => Color.White;
    public static Color CardBorder => Color.FromArgb(226, 232, 240);          // Slate 200

    // ============== STATUS BAR ==============
    public static Color StatusBarBackground => Color.FromArgb(24, 24, 37);
    public static Color StatusBarText => Color.FromArgb(226, 232, 240);
    public static Color StatusBarSuccess => Color.FromArgb(34, 197, 94);      // Green 500

    // ============== BUTTONS ==============
    public static Color PrimaryButton => Color.FromArgb(139, 92, 246);        // Violet 500
    public static Color PrimaryButtonHover => Color.FromArgb(167, 139, 250);  // Violet 400
    public static Color SuccessButton => Color.FromArgb(34, 197, 94);         // Green 500
    public static Color DangerButton => Color.FromArgb(239, 68, 68);          // Red 500
    public static Color WarningButton => Color.FromArgb(245, 158, 11);        // Amber 500
    public static Color ButtonText => Color.White;

    // ============== DATA GRID ==============
    public static Color GridHeader => Color.FromArgb(99, 102, 241);           // Indigo 500
    public static Color GridHeaderText => Color.White;
    public static Color GridAlternate => Color.FromArgb(248, 250, 252);       // Slate 50
    public static Color GridBorder => Color.FromArgb(226, 232, 240);          // Slate 200

    // ============== TEXT ==============
    public static Color TextPrimary => Color.FromArgb(30, 41, 59);            // Slate 800
    public static Color TextSecondary => Color.FromArgb(100, 116, 139);       // Slate 500
    public static Color TextTitle => Color.FromArgb(99, 102, 241);            // Indigo 500

    // ============== FONTS ==============
    public static Font FontRegular => new Font("Segoe UI", 10);
    public static Font FontBold => new Font("Segoe UI", 10, FontStyle.Bold);
    public static Font FontTitle => new Font("Segoe UI", 14, FontStyle.Bold);
    public static Font FontLarge => new Font("Segoe UI", 16, FontStyle.Bold);
    public static Font FontSmall => new Font("Segoe UI", 9);

    // ============== SIZES ==============
    public const int SidebarWidth = 220;
    public const int HeaderHeight = 50;
    public const int MenuItemHeight = 45;
    public const int StatusBarHeight = 25;
}
