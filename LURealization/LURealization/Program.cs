using LURealization;

public class Program
{
    public static void Main()
    {

        double[,] matrix = new double[,] {
            { 2, 1, 0, 0, 0, 0, 0 },
            { 1, 2, 1, 0, 0, 0, 0 },
            { 0, 1, 2, 1, 0, 0, 0 },
            { 0, 0, 1, 2, 1, 0, 0 },
            { 0, 0, 0, 1, 2, 1, 0 },
            { 0, 0, 0, 0, 1, 2, 1 },
            { 0, 0, 0, 0, 0, 1, 2 }
        };


        var matrixCore = new MatrixDecomposition().DoLuDecomposition(matrix);
    }
}   