namespace Contract
{
    /// <summary>
    /// Определяет контракт для разложения матрицы.
    /// </summary>
    public interface IMatrixDecomposition
    {
        /// <summary>
        /// LU разложение матрицы
        /// </summary>
        /// <param name="matrix">Матрица, которую нужно разложить.</param>
        /// <returns>Кортеж из двух матриц: L и U.</returns>    
        (double[,] L, double[,] U) DoLuDecomposition(double[,] matrix);
    }
}