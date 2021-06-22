using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    #region Поля и объекты
    // Mesh Filter: хранит данные меша модели.
    //Mesh Renderer: рендерит данные меша в сцене.
    //Мэш который будет генерироваться
    Mesh mesh;

    public Image image;

    public GameObject tree;
    public GameObject miniRocks;

    //Вектор вершин мэша
    Vector3[] vertices;

    // Треугольники, на которые разбивается мэш
    int[] triangles;

    // Цвета
    Color[] colors;

    //Массивы с координатами объект(деревьев, камней и пр.) на карте вершин
    public int[,] trees;
    public int[,] rocks;

    // Клоны префабов объектов
    public List<GameObject> treeClones;
    public List<GameObject> rocksClones;

    //Высоты которые генерируем;
    public float[,] heights;

    //Количество ячеек
    //Передаем из параметров меню
    private int xSize = MainMenu.valueSize;
    private int zSize = MainMenu.valueSize;

    // Градиент
    public Gradient gradient;

    //Резкость ландшафта
    //Добавляем резкость (roughness) из стартового меню
    float roughness = MainMenu.valueRoughness;

    //Высоты границ
    float maxHeight = 0;
    float minHeight = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        image.enabled = false;
        GenerateProcess();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Перезапускаем сцену
            SceneManager.LoadScene("SampleScene");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            //Быстрое сохранение
            SaveMesh();
            StartCoroutine(ShowImage());
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            //Загрузка быстрого сохранения
            LoadMesh();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            //Меш в .obj
            MeshToObj();
            StartCoroutine(ShowImage());
        }
    }

    /// <summary>
    /// Процесс генерации
    /// </summary>
    public void GenerateProcess()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    /// <summary>
    /// Обновление сцены
    /// </summary>
    public void RandomUpdate()
    {
        SceneManager.LoadScene("SampleScene");
    }


    /// <summary>
    /// Задаем вершины мэша при новой генерации
    /// </summary>
    void CreateShape()
    {

        //Очищаем мэш
        mesh.Clear();

        // Создаем объект, где хранятся данные об высотах сгенерированных методом DiamondSquare из соответствующего класса
        DiamondSquare mapHeights;
        // Задаем тут сид, если его ввел пользователь
        if (MainMenu.setSeed)
            mapHeights = new DiamondSquare(257, MainMenu.valueSeed);
        else
            mapHeights = new DiamondSquare(257);


        // Создаем вектора вершин ячеек (квадратов)
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //Массивы с координатами доп объектов
        trees = new int[zSize + 1, xSize + 1];
        rocks = new int[zSize + 1, xSize + 1];

        //Переменная нужная для записи всех вершин мэша
        int i = 0;

        //Генерируем массив с высотами методом DiamondSquare  
       mapHeights.StartGeneration(129);
       heights = mapHeights.GetArray(xSize);

        // Задаем сами вершины
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //Добавляем в вершины высоту, которую сгенерировали методом
                float y = heights.Get(z, x);
                vertices[i] = new Vector3((float)(x * 2), (float)(y * 2), (float)(z * 2));
                //heights[x,z]=y;

                //Добавляем объекты проверяя параметры высоты и расположения
                if ((z < zSize - 3) && (z > 3) && (x < xSize - 3) && (x > 3))
                {
                    // Проверка высоты
                    if ((y * 2 < maxHeight - roughness / 4))
                    {
                        //Создаем клон дерева и добавляем на мэш
                        //Quaternion.Euler - вращает объекты,а Random.Range чтобы они всегда выглядили по разному
                        if (Random.Range(0f, 1f) < 0.008f && CheckNear(trees, z, x, 5))
                        {
                            //Значение случайного размера
                            float tempScale = Random.Range(0.5f, 3f);

                            //Меняем размер дерева, которое будет склонировано
                            tree.transform.localScale = new Vector3(tempScale, tempScale, tempScale);

                            //Добавляем клон на сцену и создаем объект
                            GameObject tempTree = Instantiate(tree, new Vector3((float)(x * 2), (float)(y * 2) - 1.5f, (float)(z * 2)), Quaternion.Euler(0,
                                Random.Range(0, 360),
                                0)) as GameObject;

                            //Добавляем в список, нужно для очищения от клонов при загрузке нового меша
                            treeClones.Add(tempTree);
                        }
                    }
                    else
                    if ((y * 2 >= maxHeight - roughness / 2) && (y * 2 < maxHeight))
                    {
                        //Создаем клоны камней и добавляем на мэш
                        //Quaternion.Euler - вращает объекты,а Random.Range чтобы они всегда выглядили по разному
                        if (Random.Range(0f, 1f) < 0.009f && CheckNear(rocks, z, x, 3))
                        {
                            //Значение случайного размера
                            float tempScale = Random.Range(1.5f, 2f);

                            //Меняем размер камня, prefeb которого будет склонирован
                            miniRocks.transform.localScale = new Vector3(tempScale, tempScale, tempScale);

                            //Находим угол наклона камня AngleNear((float)mapHeights.GetValue(z, x - 1), (float)mapHeights.GetValue(z, x + 1))
                            //Добавляем клон на сцену и создаем объект
                            GameObject tempRock = Instantiate(miniRocks, new Vector3((float)(x * 2), (float)(y * 2) - 0.4f, (float)(z * 2)), Quaternion.Euler(0,
                                0,
                                AngleNear(heights.Get(z - 1, x), heights.Get(z + 1, x)))) as GameObject;

                            // Добавляем в список, нужно для очищения от клонов при загрузке нового меша
                            rocksClones.Add(tempRock);
                        }
                    }
                }
                //Записываем максимум и минимум для раскрашивания карты градиентом
                if (y > maxHeight) maxHeight = 2 * y;
                if (y < minHeight) minHeight = 2 * y;
                i++;
            }
        }

        FillShape();

    }

    /// <summary>
    /// Заполняет мэш, заполняя его треугольниками
    /// Задаем цвета из градиента в зависимости от высоты
    /// </summary>
    void FillShape()
    {
        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * zSize * 6];

        // Добавляем треугольники между вершинами
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                //Добавляем по 2 треугольника в один квадрат
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        //Создаем массив цветов,куда будем передавать цвет из градиента
        colors = new Color[vertices.Length];
        print($"Min={minHeight}; Max={maxHeight}; {Object.FindObjectsOfType(typeof(GameObject)).Length}");

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //Значение будет подогнано под значение минмимальной и максимальной высоты
                //Нужно, чтобы не выходило за пределы градиента
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                //Раскрашиваем в зависимости от высоты
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    /// <summary>
    /// Добавляем вершины, треугольники, цвета на мэш и обновляем его
    /// </summary>
    void UpdateMesh()
    {
        mesh.Clear();
        // Добавляем вершины в мэш
        mesh.vertices = vertices;
        // Добавляем полигоны в мэш
        mesh.triangles = triangles;
        // Добавялем цвета в мэш
        mesh.colors = colors;
        // Обновляем нормали
        mesh.RecalculateNormals();
        // Оптимизируем мэш встроенным в unity методом
        mesh.Optimize();
    }

    /// <summary>
    /// Проверка есть ли в радиусе от элементы подобные элементы
    /// </summary>
    /// <param name="array">массив</param>
    /// <param name="z">z координата</param>
    /// <param name="x">x координата</param>
    /// <param name="radius">радиус</param>
    /// <returns>значение правда, если есть</returns>
    bool CheckNear(int[,] array, int z, int x, int radius)
    {
        bool res = true;

        //Проверка чтобы не выйти за пределы массива
        int size1 = (array.GetLength(0) < z + radius) ? array.GetLength(0) : z + radius;
        int size2 = (array.GetLength(1) < x + radius) ? array.GetLength(1) : x + radius;

        for (int i = (0 > z - radius) ? 0 : z - radius; i < size1; i++)
        {
            for (int j = (0 > x - radius) ? 0 : x - radius; j < size2; j++)
            {
                //Проверка есть ли элемент в этом радиусе
                if (array[i, j] == 1)
                {
                    res = false;
                    break;
                }
            }
        }
        //Добавление элемента в массив при пройденной проверке
        if (res) array[z, x] = 1;
        return res;
    }

    /// <summary>
    /// Просчитываем угол наклона между двумя высотами
    /// </summary>
    /// <param name="high1">первая высота</param>
    /// <param name="high2">вторая высота</param>
    /// <returns></returns>
    public float AngleNear(float high1, float high2)
    {
        return Mathf.Atan2((Mathf.Abs(high1 - high2)), 2f) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Сохранение
    /// </summary>
    public void SaveMesh()
    {
        SaveSystem.Save(this);
        StartCoroutine(ShowImage());
    }

    /// <summary>
    /// Загрузка сохранения
    /// </summary>
    public void LoadMesh()
    {
        MeshData data = SaveSystem.LoadFile();
        if (data != null)
        {
            trees = data.trees;
            rocks = data.rocks;
            heights = data.heights;
            try
            {
                xSize = data.heights.GetLength(0) - 1;
                zSize = data.heights.GetLength(1) - 1;
            }
            catch (System.Exception)
            {
                LoadMesh();
            }
            //Очищаем сцену и вызвает метод отрисовки
            ClearScene();
            CreatLoadedMesh();
        }
    }

    /// <summary>
    /// Метод визуализации загруженного меша
    /// </summary>
    public void CreatLoadedMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


        // Создаем вектора вершин ячеек (квадратов)
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //Переменная нужная для записи всех вершин мэша
        int i = 0;

        // Задаем сами вершины
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //Добавляем в вершины высоту, которую сгенерировали методом
                float y = heights[z, x];
                vertices[i] = new Vector3((float)(x * 2), (float)(y * 2), (float)(z * 2));

                //Создаем клон дерева и добавляем на мэш
                if (trees[z, x] == 1)
                {
                    //Значение случайного размера
                    float tempScale = Random.Range(0.5f, 3f);

                    //Меняем размер дерева, которое будет склонировано
                    tree.transform.localScale = new Vector3(tempScale, tempScale, tempScale);

                    //Добавляем клон на сцену и создаем объект
                    GameObject tempTree = Instantiate(tree, new Vector3((float)(x * 2), (float)(y * 2) - 1.5f, (float)(z * 2)), Quaternion.Euler(0,
                        Random.Range(0, 360),
                        0)) as GameObject;

                    //Добавляем в список, нужно для очищения от клонов при загрузке нового меша
                    treeClones.Add(tempTree);
                }
                //Создаем клон камня и добавляем на мэш
                if (rocks[z, x] == 1)
                {
                    rocks[z, x] = 1;
                    //Значение случайного размера
                    float tempScale = Random.Range(1.5f, 2f);

                    //Меняем размер камня, prefeb которого будет склонирован
                    miniRocks.transform.localScale = new Vector3(tempScale, tempScale, tempScale);

                    //Находим угол наклона камня AngleNear((float)mapHeights.GetValue(z, x - 1), (float)mapHeights.GetValue(z, x + 1))
                    //AngleNear((float)heights[z - 1, x], (float)heights[z + 1, x])
                    //Добавляем клон на сцену и создаем объект
                    GameObject tempRock = Instantiate(miniRocks, new Vector3((float)(x * 2), (float)(y * 2) - 0.4f, (float)(z * 2)), Quaternion.Euler(0,
                        0,
                        0)) as GameObject;

                    // Добавляем в список, нужно для очищения от клонов при загрузке нового меша
                    rocksClones.Add(tempRock);
                }


                //Записываем максимум и минимум для раскрашивания карты градиентом
                if (y > maxHeight) maxHeight = 2 * y;
                if (y < minHeight) minHeight = 2 * y;
                i++;
            }
        }


        FillShape();
        UpdateMesh();
    }

    /// <summary>
    /// Очищает сцену
    /// </summary>
    public void ClearScene()
    {
        mesh.Clear();
        DeleteClones(treeClones);
        DeleteClones(rocksClones);
    }

    /// <summary>
    /// Удаляет клоны объектов на сцене
    /// </summary>
    /// <param name="clones">список объектов</param>
    public void DeleteClones(List<GameObject> clones)
    {
        for (int i = 0; i < clones.Count; i++)
        {
            Destroy(clones[i]);
        }
        clones.Clear();
    }

    /// <summary>
    /// Выводит на экран иконку сохранения при сохранении и убирает её через некоторое время
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowImage()
    {
        image.enabled = true;
        yield return new WaitForSeconds(1.5f);
        image.enabled = false;
    }

    /// <summary>
    /// Переводим сделанный мэш в obj
    /// </summary>
    public void MeshToObj()
    {
        MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
        SaveSystem.SaveInObj(viewedModelFilter);
        //Вызываем изображение
        StartCoroutine(ShowImage());
    }
}
