

using System.ComponentModel.Design;
using Contract;

namespace LURealization;

public class MatrixDecomposition : IMatrixDecomposition
{
    public (double[,] L, double[,] U) DoLuDecomposition(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double[,] L = new double[n, n];
        double[,] U = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                U[0, i] = matrix[0, i];
                L[i, 0] = matrix[i, 0] / U[0, 0];
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += L[i, k] * U[k, j];
                }

                U[i, j] = matrix[i, j] - sum;
                if (i > j)
                {
                    L[j, i] = 0;
                }
                else
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += L[j, k] * U[k, i];
                    }

                    L[j, i] = (matrix[j, i] - sum) / U[i, i];
                }
            }
        }

        return (L, U);
    }


    private static double[,] CreateMatrixWithoutColumn(double[,] matrix, int column)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1) - 1;
        double[,] result = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0, k = 0; j < cols + 1; j++)
            {
                if (j == column) continue;

                result[i, k++] = matrix[i, j];
            }
        }

        return result;
    }

    private static double[,] CreateMatrixWithoutRow(double[,] matrix, int row)
    {
        int rows = matrix.GetLength(0) - 1;
        int cols = matrix.GetLength(1);
        double[,] result = new double[rows, cols];
        for (int i = 0, k = 0; i < rows + 1; i++)
        {
            if (i == row) continue;

            for (int j = 0; j < cols; j++)
            {
                result[k, j] = matrix[i, j];
            }

            k++;
        }

        return result;
    }

    public double CalculateDeterminant(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        if (rows != cols)
        {
            throw new InvalidOperationException("Введена не квадратная матрица");
        }

        if (rows == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        double result = 0;
        object lockObj = new object();
        Parallel.For(0, cols, j =>
        {
            double minorDeterminant = CalculateDeterminant(CreateMatrixWithoutColumn(CreateMatrixWithoutRow(matrix, 0), j));
            lock (lockObj)
            {
                result += (j % 2 == 1 ? -1 : 1) * matrix[0, j] * minorDeterminant;
            }
        });

        return result;
    }
}