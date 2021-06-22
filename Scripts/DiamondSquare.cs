using System;
public class DiamondSquare
{
    //Размер всего массива высот
    public int GlobalSize { get; private set; }

    // Сид для генерации
    public int GlobalSeed { get; }

    //Рандом
    private Random rd;

    //Массив высот
    private float[,] heightMap;


    /// <summary>
    /// Конструктор короткий, передается в длинный
    /// </summary>
    /// <param name="size">Размер карты</param>
    public DiamondSquare(int size) : this(size, (int)DateTime.Now.Ticks) { }

    /// <summary>
    /// Конструктор расширенный
    /// </summary>
    /// <param name="size">Размер карты</param>
    /// <param name="seed">Сид генерации мира</param>
    public DiamondSquare(int size, int seed)
    {
        //  Инициализируем значения и 4 вершины массива
        GlobalSeed = seed;
        GlobalSize = PowerOfTwo(size);
        rd = new Random(GlobalSeed);
        //Создаем карту высот заданного размера
        heightMap = new float[GlobalSize, GlobalSize];

        //Добавляем случайные значения на вершины карты (квадрата)
        heightMap[0, 0] = RandomValue();
        heightMap[size - 1, 0] = RandomValue();
        heightMap[size - 1, size - 1] = RandomValue();
        heightMap[0, size - 1] = RandomValue();
    }

    /// <summary>
    /// Генерируем карту, используя diamond square
    /// Будет запускаться со стандартным значением длины
    /// </summary>
    public void StartGeneration()
    {

        // Количество шагов отрисовки, чем больше, тем больше точность, нагрузка и плавность.
        int currentSize = GlobalSize;
        //Разрешение отвечает за плавность рельефа, чем больше, тем меньше локальных неровностей
        float scale = 1.0f;

        while (currentSize > 1)
        {
            //вызываем шаг для каждого квадрата, в данном случае шаг- это передаваемый размер
            GenerationStep(currentSize, scale);

            //Уменьшаем размер в 2 раза
            currentSize >>= 1;
            scale /= 2.0f;
        }
    }

    /// <summary>
    /// Генерируем карту, используя diamond square
    /// </summary>
    /// <param name="size">размер генерации</param>
    public void StartGeneration(int size)
    {

        // Количество шагов отрисовки, чем больше, тем больше точность, нагрузка и плавность.
        int currentSize = PowerOfTwo(size);
        //Разрешение отвечает за плавность рельефа, чем больше, тем меньше локальных неровностей
        float scale = 1.0f;

        while (currentSize > 1)
        {
            //вызываем шаг для каждого квадрата, в данном случае шаг- это передаваемый размер
            GenerationStep(currentSize, scale);

            //Уменьшаем размер в 2 раза
            currentSize >>= 1;
            scale /= 2.0f;
        }
    }

    /// <summary>
    /// Запуск отрисовки следующего шага
    /// </summary>
    /// <param name="size">шаг который делаем</param>
    /// <param name="scale">разрешение</param>
    private void GenerationStep(int size, float scale)
    {
        // Квадраты
        for (int y = size / 2; y < size / 2 + GlobalSize; y += size)
        {
            for (int x = size / 2; x < size / 2 + GlobalSize; x += size)
            {
                Square(x, y, size, RandomValue() * scale);
            }
        }

        // Ромбы
        for (int y = 0; y < GlobalSize; y += size)
        {
            for (int x = 0; x < GlobalSize; x += size)
            {
                Diamond(x + size / 2, y, size, RandomValue() * scale);
                Diamond(x, y + size / 2, size, RandomValue() * scale);
            }
        }
    }

    /// <summary>
    /// Находим значение для точки (x, y) в середине квадрата,
    /// добавляя среднее арифметическое вершин квадрата и случайное значение
    /// </summary>
    /// <param name="x">x координата</param>
    /// <param name="y">y координата</param>
    /// <param name="size">шаг</param>
    /// <param name="randomNumber">случайное значение, которое добавим к точке</param>
    private void Square(int x, int y, int size, float randomNumber)
    {
        int halfSize = size / 2;

        //Вершины квадрата
        float a = heightMap.Get(x - halfSize, y - halfSize);
        float b = heightMap.Get(x + halfSize, y - halfSize);
        float c = heightMap.Get(x - halfSize, y + halfSize);
        float d = heightMap.Get(x + halfSize, y + halfSize);

        // Добавляем случайное значение в точку в центре, учитывая среднее арифметическое вершин квадрата
        heightMap.Set(x, y, (a + b + c + d) / 4.0f + randomNumber);
    }

    /// <summary>
    /// Находим значение для точки (x, y) в середине робма
    /// добавляя среднее арифметическое вершин ромба и случайное значение
    /// </summary>
    /// <param name="x">x координата</param>
    /// <param name="y">y координата.</param>
    /// <param name="size">шаг</param>
    /// <param name="randomNumber">случайное значение, которое добавим к точке</param>
    private void Diamond(int x, int y, int size, float randomNumber)
    {
        int halfSize = size / 2;

        //Вершины ромба
        float b = heightMap.Get(x + halfSize, y);
        float d = heightMap.Get(x - halfSize, y);
        float a = heightMap.Get(x, y - halfSize);
        float c = heightMap.Get(x, y + halfSize);

        // Добавляем случайное значение в точку в центре
        heightMap.Set(x, y, (a + b + c + d) / 4.0f + randomNumber);
    }


    /// <summary>
    /// Возвращает значение от -1 до 1, нужно для плавного перехода высот
    /// </summary>
    /// <returns>случайное значение</returns>
    private float RandomValue()
    {
        return (float)(rd.NextDouble() * 2 - 1);
    }

    /// <summary>
    /// Заменяем число на ближайшую степень двойки
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static int PowerOfTwo(int value)
    {
        int newValue = 1;
        while (newValue < value)
            newValue = newValue * 2;
        return newValue;
    }

    /// <summary>
    /// Возвращает массив заданной длины с учетом резкости ландшафта
    /// </summary>
    /// <returns>двумерный массив float</returns>
    public float[,] GetArray(int size)
    {
        float[,] array = new float[size + 1, size + 1];
        for (int x = 0; x <= size; x++)
        {
            for (int y = 0; y <= size; y++)
            {
                array[y, x] = heightMap.Get(x, y) * MainMenu.valueRoughness;
            }
        }

        return array;
    }
}
