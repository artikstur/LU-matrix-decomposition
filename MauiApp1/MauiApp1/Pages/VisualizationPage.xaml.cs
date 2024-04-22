using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages;

public partial class VisualizationPage : ContentPage
{
    private int[,] _matrix;
    public VisualizationPage(int[,] matrix)
    {
        _matrix = matrix;
        InitializeComponent();
        Title = "Визуализация";
    }
}