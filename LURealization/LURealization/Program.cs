using LURealization;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        int n = 4000;
        double[,] matrix = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = Math.Pow(i + j + 1, 2); 
            }
        }

        var matrixCore = new MatrixDecomposition();
    
        Stopwatch parallelStopwatch = new Stopwatch();
        parallelStopwatch.Start();
        matrixCore.DoLuDecomposition(matrix);
        parallelStopwatch.Stop();
        TimeSpan parallelTime = parallelStopwatch.Elapsed;

        Stopwatch syncStopwatch = new Stopwatch();
        syncStopwatch.Start();
        matrixCore.SyncDecomposition(matrix);
        syncStopwatch.Stop();
        TimeSpan syncTime = syncStopwatch.Elapsed;

        Console.WriteLine($"{n}:");
        Console.WriteLine("Время выполнения параллельного метода: " + parallelTime.TotalMilliseconds);
        Console.WriteLine("Время выполнения синхронного метода: " + syncTime.TotalMilliseconds);
    }
}   