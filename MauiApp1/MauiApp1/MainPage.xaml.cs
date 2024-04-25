using System.Security.AccessControl;
using System.Xml;
using MauiApp1.Pages;
using System.Reflection;
using Contract;
using Microsoft.Maui.Platform;



namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private int _dimension;
        private string _path;
        private Assembly _currentAssembly;
        private bool _assemblyStatus;
        private Button _dllButton = new ()
        {
            Text = "Загрузить сборку",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            WidthRequest = 200,
            HeightRequest = 60,
            FontSize = 20,
        };

        public MainPage()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void CreateEntry()
        {
            Entry entry = new Entry()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                WidthRequest = 50,
                HeightRequest = 50,
                MaxLength = 100,
                BackgroundColor = Colors.GhostWhite,
            };
            entry.TextChanged += EntryTextChanged;

            MainLayout.Children.Add(entry);
        }
        private void InitializeGrid()
        {
            CreateEntry();
            AddMainButton();
            AddGetDllButton();
        }

        private void AddGetDllButton()
        {
            _dllButton.BackgroundColor = Colors.Red;
            _dllButton.SetDynamicResource(Button.TextColorProperty, "TextColor");

            _dllButton.Clicked += (sender, e) =>
            {
                PickDllFile();
            };

            MainLayout.Children.Add(_dllButton);
        }

        private void AddMainButton()
        {
            Button mainButton = new Button()
            {
                Text = "Далее",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 200,
                HeightRequest = 60,
                FontSize = 18,
            };

            mainButton.SetDynamicResource(Button.BackgroundProperty, "MySecondary");
            mainButton.SetDynamicResource(Button.TextColorProperty, "TextColor");
            mainButton.Clicked += (sender, e) =>
            {
                ToPage();
            };

            MainLayout.Children.Add(mainButton);
        }

        private async Task PickDllFile()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".dll" } },
                });

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions()
                {
                    PickerTitle = "Выберете сборку",
                    FileTypes = customFileType,
                });

                _path = result.FullPath;
                UploadNewDll();
            }
            catch
            {
                Console.WriteLine("Ошибка");
            }
        }

        private void UploadNewDll()
        {
            Assembly asm = Assembly.LoadFrom(_path);
            _currentAssembly = asm;
            CheckForContract(asm);
        }

        private void CheckForContract(Assembly asm)
        {
            Type[] types = asm.GetTypes();

            bool hasImplementation = types.Any(t => typeof(IMatrixDecomposition).IsAssignableFrom(t) && t.IsClass);

            _assemblyStatus = hasImplementation;
            if (_assemblyStatus)
            {
                _dllButton.BackgroundColor = Colors.Green;
            }
            else
            {
                DisplayAlert("Ошибка", "Не удалось найти реализацию IMatrixDecomposition. Загрузите правильную сборку.", "OK");
            }
        }
        private void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;
            if (int.TryParse(e.NewTextValue, out int result))
            {
                _dimension = result;
            }
        }

        private async void ToPage()
        {
            if (_dimension > 2)
            {
                if (_assemblyStatus)
                {
                    await Navigation.PushAsync(new EntryMatrixPage(_dimension, _currentAssembly), true);
                }
                else
                {
                    DisplayAlert("Ошибка", "Не удалось найти реализацию IMatrixDecomposition. Загрузите правильную сборку.", "OK");
                }
            }
            else
            {
                DisplayAlert("Ошибка", "Матрицы слишком мала. Введите размерность минимум: 3", "OK");
            }
        }

        private void ChangeTheme(object sender, EventArgs e)
        {
            if (ThemeManager.SelectedTheme == nameof(MauiApp1.Resources.Themes.Dark))
            {
                ThemeManager.SetTheme(nameof(MauiApp1.Resources.Themes.Default));
            }
            else
            {
                ThemeManager.SetTheme(nameof(MauiApp1.Resources.Themes.Dark));
            }

        }
    }
}
