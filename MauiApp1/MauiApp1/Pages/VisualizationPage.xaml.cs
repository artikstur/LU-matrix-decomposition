using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages;

public partial class VisualizationPage : ContentPage
{
    private double[,] _matrix;
    private int n;
    private Assembly _asm;
    private double[,]? lowerMatrix;
    private double[,]? upperMatrix;
    private Label timerLabel = new Label()
    {
        TextColor = Colors.White
    };

    private ActivityIndicator activityIndicator = new ActivityIndicator()
    {
        IsRunning = true,
        HeightRequest = 100,
        WidthRequest = 100,
    };

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    public VisualizationPage(double[,] matrix, Assembly asm)
    {
        _matrix = matrix;
        n = _matrix.GetLength(0);
        _asm = asm;
        InitializeComponent();
        AddActivityCircle();
        _ = GetAsync();
    }

    private void AddActivityCircle()
    {
        VisualizationLayout.Children.Add(activityIndicator);
    }
    private async Task GetAsync()
    {
        await Task.Delay(500);
        await Task.Run(() =>
        {
            GetDataFromDll();
        });
    }

    private void AddChangeThemeButton()
    {
        Button button = new Button()
        {
            Text = "Тема",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            WidthRequest = 200,
            HeightRequest = 60,
            FontSize = 18,
            Margin = 30,
            TextColor = Colors.White,
        };

        button.SetDynamicResource(BackgroundProperty, "ChangeTheme");

        button.Clicked += ChangeTheme;

        VisualizationLayout.Children.Add(button);
    }

    private void AddGoBackButton()
    {
        Button button = new Button()
        {
            Text = "Назад",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            WidthRequest = 200,
            HeightRequest = 60,
            FontSize = 18,
            Margin = 30,
        };

        button.SetDynamicResource(BackgroundProperty, "MyTertiary");
        button.SetDynamicResource(Button.TextColorProperty, "TextColor");

        button.Clicked += ToMainPage;

        VisualizationLayout.Children.Add(button);
    }

    private void InitializeGrid()
    {
        var res = (lowerMatrix, upperMatrix);

        if (lowerMatrix == null || upperMatrix == null)
        {
            AddGoBackButton();
            DisplayAlert("Ошибка", "Данную матрицу не разложить", "Ок");
            return;
        }

        Grid lGrid = new Grid() { };
        Grid uGrid = new Grid() { };

        Frame lFrame = new Frame
        {
            Margin = 30,
            Content = lGrid,
            CornerRadius = 10,
            HasShadow = true,
            Padding = 15,
            WidthRequest = n * 90,
            HeightRequest = n * 90,
            BackgroundColor = Color.FromArgb("#FFFF99"),
        };

        lFrame.SetDynamicResource(BackgroundProperty, "MySecondary");
        Frame uFrame = new Frame
        {
            Margin = 30,
            Content = uGrid,
            CornerRadius = 10,
            HasShadow = true,
            WidthRequest = n * 90,
            HeightRequest = n * 90,
            BackgroundColor = Color.FromArgb("#4682B4"),
        };
        uFrame.SetDynamicResource(BackgroundProperty, "MySecondary");

        for (int i = 0; i < n; i++)
        {
            lGrid.RowDefinitions.Add(new RowDefinition());
            lGrid.ColumnDefinitions.Add(new ColumnDefinition());

            uGrid.RowDefinitions.Add(new RowDefinition());
            uGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }


        for (int i = 0; i < res.Item1.GetLength(0); i++)
        {
            for (int j = 0; j < res.Item1.GetLength(1); j++)
            {
                Label LmatrixLabel = new Label()
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = res.Item1[i, j].ToString("F2"),
                    FontSize = 20,
                    WidthRequest = 80,
                    HeightRequest = 80,
                };

                Label UmatrixLabel = new Label()
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = res.Item2[i, j].ToString("F2"),
                    FontSize = 20,
                    WidthRequest = 80,
                    HeightRequest = 80,
                };

                lGrid.Children.Add(LmatrixLabel);
                lGrid.SetRow(LmatrixLabel, i);
                lGrid.SetColumn(LmatrixLabel, j);

                uGrid.Children.Add(UmatrixLabel);
                uGrid.SetRow(UmatrixLabel, i);
                uGrid.SetColumn(UmatrixLabel, j);
            }
        }

        Label lTextLabel = new Label
        {
            Text = "L:",
            FontSize = 30,
            HorizontalOptions = LayoutOptions.Center,
        };

        Label uTextLabel = new Label
        {
            Text = "U:",
            FontSize = 30,
            HorizontalOptions = LayoutOptions.Center,
        };

        var lStackLayout = new StackLayout
        {
            Children = { lTextLabel, lFrame }
        };

        var uStackLayout = new StackLayout
        {
            Children = { uTextLabel, uFrame }
        };

        lTextLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");
        uTextLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");

        VisualizationLayout.Children.Add(lStackLayout);
        VisualizationLayout.Children.Add(uStackLayout);

        VisualizationLayout.Children.Remove(activityIndicator);
    }

    private void GetDataFromDll()
    {
        Type? matrixDecompositionType = _asm.GetTypes().FirstOrDefault(t => t.Name == "MatrixDecomposition");

        var matrixDecompositionInstance = Activator.CreateInstance(matrixDecompositionType);

        MethodInfo? method = matrixDecompositionType.GetMethod("DoLuDecomposition");

        var result = (ValueTuple<double[,], double[,]>)(method.Invoke(matrixDecompositionInstance, new object[] { _matrix }));

        lowerMatrix = result.Item1;
        upperMatrix = result.Item2;

        MainThread.BeginInvokeOnMainThread(InitializeGrid);
  
    }

    private async void ToMainPage(object sender, EventArgs e)
    {
        await Task.Delay(100);
        await Navigation.PopAsync();
    }

    private async void StartTimer()
    {
        int seconds = 0;
        VisualizationLayout.Children.Add(timerLabel);
        while (true)
        {
            timerLabel.Text = $"Время: {seconds++} сек";

            await Task.Delay(1000);
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