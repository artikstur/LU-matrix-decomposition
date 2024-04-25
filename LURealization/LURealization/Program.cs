using LURealization;

public class Program
{
    public static void Main()
    {

        double[,] matrix = new double[,] {
            { 123, 456, 789, 0, 0, 0, 0 },
            { 987, 654, 321, 123, 0, 0, 0 },
            { 456, 789, 101, 234, 567, 0, 0 },
            { 888, 222, 333, 444, 555, 666, 0 },
            { 111, 222, 333, 444, 555, 666, 777 },
            { 999, 888, 777, 666, 555, 444, 333 },
            { 123, 321, 456, 654, 789, 987, 101 }
        };



        var matrixCore = new MatrixDecomposition().DoLuDecomposition(matrix);
    }
}   