

using System.ComponentModel.Design;
using System.Drawing;
using Contract;

namespace LURealization;

public class MatrixDecomposition : IMatrixDecomposition
{
    public (double[,] L, double[,] U) DoLuDecomposition(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double[,] L = new double[n, n];
        double[,] U = new double[n, n];

        //if (CalculateDeterminant(matrix) == 0)
        //{
        //    return (null, null);
        //}

        for (int i = 0; i < n; i++)
        {
            L[i, i] = 1;

            Parallel.For(i, n, j =>
            {
                if (j < i)
                {
                    L[j, i] = 0;
                    return;
                }

                U[i, j] = matrix[i, j];
                for (int k = 0; k < i; k++)
                {
                    U[i, j] -= L[i, k] * U[k, j];
                }
            });
            
            Parallel.For(i + 1, n, j =>
            {
                if (j < i || U[i, i] == 0)
                {
                    return;
                }

                L[j, i] = matrix[j, i] / U[i, i];
                for (int k = 0; k < i; k++)
                {
                    L[j, i] -= L[j, k] * U[k, i] / U[i, i];
                }
            });
        }


        //PrintMatrix(matrix);
        //Console.WriteLine();
        //PrintMatrix(L);
        //Console.WriteLine();
        //PrintMatrix(U);
        //Console.WriteLine();
        //PrintMatrix(MatrixMultiply(L, U));
        //Console.WriteLine();
        //PrintMatrix(MatrixMultiplyNotParallel(L, U));

        //if (!AreMatricesEqual(matrix, MatrixMultiply(L, U)))
        //{
        //    throw new Exception("Разложение выполнено неверно!!!");
        //}

        return (L, U);
    }

    public void SyncDecomposition(double[,] matrix)
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
    }

    public void ParallelDecomposition(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double[,] L = new double[n, n];
        double[,] U = new double[n, n];

        Parallel.For(0, n, i =>
        {
            // Расчет верхней треугольной матрицы U
            for (int j = i; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += L[i, k] * U[k, j];
                }
                U[i, j] = matrix[i, j] - sum;
            }

            // Расчет нижней треугольной матрицы L
            for (int j = i + 1; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += L[j, k] * U[k, i];
                }
                L[j, i] = (matrix[j, i] - sum) / U[i, i];
            }

            // Установка диагональных элементов L в 1
            L[i, i] = 1;
        });
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
        Parallel.For(0, cols, j =>
        {
            double minorDeterminant = CalculateDeterminant(CreateMatrixWithoutColumn(CreateMatrixWithoutRow(matrix, 0), j));
            result += (j % 2 == 1 ? -1 : 1) * matrix[0, j] * minorDeterminant;
        });

        return result;
    }

    private static double[,] MatrixMultiply(double[,] matrixA, double[,] matrixB)
    {
        int aRows = matrixA.GetLength(0);
        int aCols = matrixA.GetLength(1);
        int bRows = matrixB.GetLength(0);
        int bCols = matrixB.GetLength(1);

        if (aCols != bRows)
        {
            throw new ArgumentException("Невозможно перемножить");
        }

        double[,] result = new double[aRows, bCols];

        Parallel.For(0, aRows, i =>
            {
                for (int j = 0; j < bCols; ++j)
                {
                    for (int k = 0; k < aCols; ++k)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }
        );

        return result;
    }

    static void PrintMatrix(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(matrix[i, j].ToString("F2") + " "); // Отображение с двумя знаками после запятой
            }
            Console.WriteLine();
        }
    }

    private static double[,] MatrixMultiplyNotParallel(double[,] matrixA, double[,] matrixB)
    {
        int aRows = matrixA.GetLength(0);
        int aCols = matrixA.GetLength(1);
        int bRows = matrixB.GetLength(0);
        int bCols = matrixB.GetLength(1);

        if (aCols != bRows)
        {
            throw new ArgumentException("Невозможно перемножить");
        }

        double[,] result = new double[aRows, bCols];

        Parallel.For(0, aRows, i =>
        {
            for (int j = 0; j < bCols; ++j)
            {
                for (int k = 0; k < aCols; ++k)
                {
                    result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }
        });

        return result;
    }

    static bool AreMatricesEqual(double[,] matrix1, double[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
        {
            return false;
        }

        int rows = matrix1.GetLength(0);
        int cols = matrix1.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (Math.Abs(matrix1[i, j] - matrix2[i, j]) > 0.1)
                {
                    Console.WriteLine(matrix1[i, j]);
                    Console.WriteLine(matrix2[i, j]);
                    return false; 
                }
            }
        }

        return true; 
    }
}
