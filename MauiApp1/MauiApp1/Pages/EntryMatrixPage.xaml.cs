using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages;

public partial class EntryMatrixPage : ContentPage
{
    private Assembly _asm;
    private int _dimension;
    private Grid _mainGrid;
    private readonly List<Entry> _entries = new List<Entry>();
    private double[,] _matrix;
    public EntryMatrixPage(int dimension, Assembly asm)
    {
        _asm = asm;
        _dimension = dimension;
        InitializeComponent();
        InitializeGrid(dimension);
    }

    private void InitializeGrid(int n)
    {
        _matrix = new double[_dimension, _dimension];
        _mainGrid = new Grid
        {
            ColumnSpacing = 5,
            RowSpacing = 5,
            WidthRequest = n * 60,
            HeightRequest = n * 60,
        };


        Frame frame = new Frame
        {
            Margin = 20,
            Content = _mainGrid,
            CornerRadius = 10,
            HasShadow = true,
            Padding = 15,
            WidthRequest = n * 70,
            HeightRequest = n * 70,
            BackgroundColor = Color.FromArgb("#E14845"),
        };

        frame.SetDynamicResource(BackgroundProperty, "MyTertiary");

        EntryPageLayout.Add(frame);

        for (int i = 0; i < n; i++)
        {
            _mainGrid.RowDefinitions.Add(new RowDefinition());
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        SetInitialContent(n);
        AddMainButton();
    }

    private void AddMainButton()
    {
        Button mainButton = new Button()
        {
            Text = "Разложить",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            WidthRequest = 200,
            HeightRequest = 60,
            FontSize = 18,
            Margin = 30,

        };

        mainButton.SetDynamicResource(BackgroundProperty, "MySecondary");
        mainButton.SetDynamicResource(Button.TextColorProperty, "TextColor");

        mainButton.Clicked += (sender, e) =>
        {
            bool allFieldsFilled = _entries.All(entry => !string.IsNullOrEmpty(entry.Text));

            if (allFieldsFilled)
            {
                for (int i = 0; i < _dimension; i++)
                {
                    for (int j = 0; j < _dimension; j++)
                    {
                        _matrix[i, j] = double.Parse(_entries[i * _dimension + j].Text);
                    }
                }
                ToPage();
            }
            else
            {
                DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
            }
        };

        EntryPageLayout.Children.Add(mainButton);
    }


    private void SetInitialContent(int n)
    {
        for (int row = 0; row < n; row++)
        {
            for (int column = 0; column < n; column++)
            {
                Entry entry = new Entry()
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 16,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    MaxLength = 5,
                    BackgroundColor = Colors.GhostWhite,
                };
                
                entry.TextChanged += EntryTextChanged;

                _entries.Add(entry);
                _mainGrid.Children.Add(entry);

                Grid.SetRow(entry, row);
                Grid.SetColumn(entry, column);
            }
        }
    }

    private void EntryTextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = (Entry)sender;
        string newText = e.NewTextValue;

        if (!string.IsNullOrEmpty(newText))
        {
            newText = string.Join("", newText.Where(char.IsDigit));
            entry.Text = newText;
        }

    }

    private async void ToPage()
    {
        await Navigation.PushAsync(new VisualizationPage(_matrix, _asm));
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