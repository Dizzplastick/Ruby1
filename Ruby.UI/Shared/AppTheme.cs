using MudBlazor;

namespace Ruby.UI.Shared
{
    public static class AppTheme
    {
        public static MudTheme myTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#1db954", // Например, зеленый как в Spotify
                Secondary = "#b3b3b3",
                AppbarBackground = "#ffffff"
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#9b0000", 
                Secondary = "#a0a0a0",
                Background = "#121212", 
                Surface = "#1e1e1e", 
                TextPrimary = "#ffffff", 
                TextSecondary = "#b3b3b3", 
                DrawerBackground = "#000000" 
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "8px", 
            },
            Typography = new Typography()
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "Work Sans", "sans-serif" },
                    FontWeight = "800"
                }
            }
        };
    };
}
