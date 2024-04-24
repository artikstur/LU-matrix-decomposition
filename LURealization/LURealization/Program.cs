﻿using LURealization;

public class Program
{
    public static void Main()
    {

        double[,] matrix = new double[,] {
            { 4, 2, 1 },
            { 7, 5, 10 },
            { 9, 8, 6 }
        };

        var matrixCore = new MatrixDecomposition().DoLuDecomposition(matrix);
    }
}   