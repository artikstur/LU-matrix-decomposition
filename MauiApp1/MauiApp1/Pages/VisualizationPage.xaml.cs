﻿using Microsoft.Maui.Platform;
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

    public VisualizationPage(double[,] matrix, Assembly asm)
    {
        _matrix = matrix;
        n = _matrix.GetLength(0);
        _asm = asm;
        InitializeComponent();
        GetDataFromDll();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        var res = (lowerMatrix, upperMatrix);

        if (lowerMatrix == null || upperMatrix == null)
        {
            DisplayAlert("Ошибка", "Данную матрицу невозможно разложить", "OK");
            ToMainPage();
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
    }

    private void GetDataFromDll()
    {
        Type? matrixDecompositionType = _asm.GetTypes().FirstOrDefault(t => t.Name == "MatrixDecomposition");

        var matrixDecompositionInstance = Activator.CreateInstance(matrixDecompositionType);

        MethodInfo? method = matrixDecompositionType.GetMethod("DoLuDecomposition");

        var result = (ValueTuple<double[,], double[,]>)method.Invoke(matrixDecompositionInstance, new object[] { _matrix });

        lowerMatrix = result.Item1;
        upperMatrix = result.Item2;
    }


    private async void ToMainPage()
    {
        await Task.Delay(1000);
        await Navigation.PopAsync();
    }

}