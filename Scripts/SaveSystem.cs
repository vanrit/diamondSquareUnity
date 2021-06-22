using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/// <summary>
/// Класс с системой сохранения
/// </summary>
public class SaveSystem
{
    /// <summary>
    /// Сохранение в файл
    /// </summary>
    /// <param name="myMesh"></param>
    public static void Save(MeshGenerator myMesh)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/genericMap.mesh";
            FileStream stream = new FileStream(path, FileMode.Create);


            MeshData data = new MeshData(myMesh);

            formatter.Serialize(stream, data);
            stream.Close();

            //File.WriteAllText(Application.persistentDataPath + " /array.txt", testText);
        }
        catch (System.Exception)
        {
        }

    }

    /// <summary>
    /// Загрузка последнего сохранения
    /// </summary>
    /// <returns></returns>
    public static MeshData LoadFile()
    {
        try
        {
            string path = Application.persistentDataPath + "/genericMap.mesh";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MeshData data = formatter.Deserialize(stream) as MeshData;
            return data;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Сохраняем мэш в obj
    /// </summary>
    /// <param name="meshFilter"></param>
    public static void SaveInObj(MeshFilter meshFilter)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "data");
            path = Path.Combine(path, "model" + ".obj");

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            ObjExporter.MeshToFile(meshFilter, path);
        }
        catch (System.Exception)
        {
        }
    }


}
