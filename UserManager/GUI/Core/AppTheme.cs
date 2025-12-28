namespace UserManager.GUI.Core;

/// <summary>
/// Định nghĩa các màu sắc và style cho toàn bộ ứng dụng
/// </summary>
public static class AppTheme
{
    // ============== SIDEBAR ==============
    /// <summary>Màu nền sidebar chính</summary>
    public static Color SidebarBackground => Color.FromArgb(44, 62, 80);      // Dark blue-gray
    
    /// <summary>Màu nền khi hover menu item</summary>
    public static Color SidebarHover => Color.FromArgb(52, 73, 94);           // Lighter hover
    
    /// <summary>Màu nền menu item đang active</summary>
    public static Color SidebarActive => Color.FromArgb(41, 128, 185);        // Blue highlight
    
    /// <summary>Màu chữ trên sidebar</summary>
    public static Color SidebarText => Color.FromArgb(236, 240, 241);         // Light gray
    
    /// <summary>Màu chữ mờ hơn</summary>
    public static Color SidebarTextMuted => Color.FromArgb(149, 165, 166);    // Muted gray

    // ============== HEADER ==============
    public static Color HeaderBackground => Color.FromArgb(52, 73, 94);       // Slightly lighter than sidebar
    public static Color HeaderText => Color.White;

    // ============== CONTENT AREA ==============
    public static Color ContentBackground => Color.FromArgb(236, 240, 241);   // Light gray
    public static Color CardBackground => Color.White;
    public static Color CardBorder => Color.FromArgb(189, 195, 199);

    // ============== STATUS BAR ==============
    public static Color StatusBarBackground => Color.FromArgb(44, 62, 80);
    public static Color StatusBarText => Color.FromArgb(236, 240, 241);
    public static Color StatusBarSuccess => Color.FromArgb(46, 204, 113);     // Green

    // ============== BUTTONS ==============
    public static Color PrimaryButton => Color.FromArgb(41, 128, 185);        // Blue
    public static Color PrimaryButtonHover => Color.FromArgb(52, 152, 219);   // Lighter blue
    public static Color SuccessButton => Color.FromArgb(39, 174, 96);         // Green
    public static Color DangerButton => Color.FromArgb(231, 76, 60);          // Red
    public static Color WarningButton => Color.FromArgb(243, 156, 18);        // Orange
    public static Color ButtonText => Color.White;

    // ============== DATA GRID ==============
    public static Color GridHeader => Color.FromArgb(41, 128, 185);           // Blue
    public static Color GridHeaderText => Color.White;
    public static Color GridAlternate => Color.FromArgb(245, 247, 250);       // Very light gray
    public static Color GridBorder => Color.FromArgb(189, 195, 199);

    // ============== TEXT ==============
    public static Color TextPrimary => Color.FromArgb(44, 62, 80);            // Dark
    public static Color TextSecondary => Color.FromArgb(127, 140, 141);       // Gray
    public static Color TextTitle => Color.FromArgb(41, 128, 185);            // Blue

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
