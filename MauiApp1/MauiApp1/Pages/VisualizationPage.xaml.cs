using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages;

public partial class VisualizationPage : ContentPage
{
    private int[,] _matrix;
    private Assembly _asm;
    private Grid _mainGrid;

    public VisualizationPage(int[,] matrix, Assembly asm)
    {
        _matrix = matrix;
        _asm = asm;
        InitializeComponent();
    }

    private void InitializeGrid(int n)
    { 
        //VisualizationLayout.Children.Add(_mainGrid);
    }
}