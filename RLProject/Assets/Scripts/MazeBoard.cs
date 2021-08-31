using System.Threading.Tasks;
using UnityEngine;

public class MazeBoard : MonoBehaviour
{
    public static readonly int HEIGHT = 12;
    public static readonly int WIDTH = 20;

    private MazeNode[,] mazeNodes;

    [SerializeField] GameObject prefabMazeNode;

    // Start is called before the first frame update
    void Start()
    {
        mazeNodes = new MazeNode[HEIGHT,WIDTH];

        for (int row = 0; row < HEIGHT; ++row)
        {
            for(int col = 0; col < WIDTH; ++col)
            {
                var mazeNode = Instantiate(prefabMazeNode);

                var transform = mazeNode.GetComponent<Transform>();

                transform.SetParent(GetComponent<Transform>());

                transform.position = GetPosition(row, col);

                mazeNodes[row, col] = mazeNode.GetComponent<MazeNode>();
            }
        }

        SetIsChangeable(true);

        Invoke("doing", 30);
    }

    private void doing()
    {
        SetIsChangeable(false);
        Algorithm alg = (new GameObject("BFS Algorithm")).AddComponent<BFS>();
        Debug.Log(alg.IsSolvable(0, 0, mazeNodes));
    }

    public void SetIsChangeable(bool b)
    {
        MazeNode.SetIsChangeable(b);
    }

    public static Vector3 GetPosition(int row, int col)
    {
        return new Vector3((((float)-WIDTH) / 2 + col + 0.5f) * MazeNode.WIDTH, ((float)HEIGHT / 2 - row - 0.5f) * MazeNode.HEIGHT);
    }
}
