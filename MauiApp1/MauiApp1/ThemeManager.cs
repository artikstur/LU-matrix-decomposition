namespace MauiApp1;

public static class ThemeManager
{
    private static readonly IDictionary<string, ResourceDictionary> _themes = new Dictionary<string, ResourceDictionary>()
    {
        [nameof(Resources.Themes.Default)] = new Resources.Themes.Default(),
        [nameof(Resources.Themes.Dark)] = new Resources.Themes.Dark(),
    };

    public static string? SelectedTheme { get; set; }

    public static async Task SetTheme(string themeName)
    {
        if (SelectedTheme == themeName)
        {
            return;
        }

        var themeToBeApplied = _themes[themeName];


        Task.Run(() =>
        {
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(themeToBeApplied);
                Application.Current.Resources.MergedDictionaries.Add(new Resources.Styles.EntryStyle());

                SelectedTheme = themeName;
            });
        });
    }
}