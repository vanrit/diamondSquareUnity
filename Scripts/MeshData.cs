

[System.Serializable]
public class MeshData
{
    public float[,] heights;
    public int[,] trees;
    public int[,] rocks;

    public MeshData(MeshGenerator myMesh)
    {
        heights = myMesh.heights;
        trees = myMesh.trees;
        rocks = myMesh.rocks;
    }
}
