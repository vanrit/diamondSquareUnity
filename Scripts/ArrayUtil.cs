/// <summary>
/// Класс с расширениями для массивов
/// </summary>
static class ArrayUtil
{
    /// <summary>
    /// Находит отражение для значения относительно максимального
    /// То есть, чтобы значение не выходило за рамки
    /// </summary>
    /// <param name="value">значение</param>
    /// <param name="max">максимальный размер, граница</param>
    /// <returns></returns>
    private static int Fit(this int value, int max)
    {

        if (value >= 0 && value < max)
            return value;
        int newValue = value % (2 * max);
        if (newValue < 0)
            newValue += 2 * max;// если меньше 0 прибавляем
        if (newValue >= max)
            newValue = 2 * max - newValue;//если больше максимального отнимаем
        return newValue;
    }

    /// <summary>
    /// Возвращает значение из массива, даже если оно выходит за его пределы
    /// </summary>
    /// <typeparam name="T">тип</typeparam>
    /// <param name="array">массив</param>
    /// <param name="x">координата x</param>
    /// <param name="y">координата y</param>
    /// <returns></returns>
    public static T Get<T>(this T[,] array, int x, int y)
    {
        return array[x.Fit(array.GetLength(0) - 1), y.Fit(array.GetLength(1) - 1)];
    }

    /// <summary>
    /// Добавляет значение в массив, даже если координаты выходят за его пределы
    /// </summary>
    /// <typeparam name="T">тип</typeparam>
    /// <param name="array">массив</param>
    /// <param name="x">координата x</param>
    /// <param name="y">координата y</param>
    /// <param name="value">значение, которое передаем</param>
    public static void Set<T>(this T[,] array, int x, int y, T value)
    {
        array[x.Fit(array.GetLength(0) - 1), y.Fit(array.GetLength(1) - 1)] = value;
    }
}
