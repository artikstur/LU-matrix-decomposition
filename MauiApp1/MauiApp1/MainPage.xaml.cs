
using MauiApp1.ViewModel;
using System.Security.AccessControl;
using System.Xml;
using MauiApp1.Pages;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private int _dimension;
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

            mainButton.Clicked += (sender, e) =>
            {
                ToPage();
            };

            MainLayout.Children.Add(mainButton);
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
                await Navigation.PushAsync(new EntryMatrixPage(_dimension));
            }
            else
            {
                DisplayAlert("Ошибка", "Матрицы слишком мала. Введите размерность минимум: 3", "OK");
            }
        }
    }
}
