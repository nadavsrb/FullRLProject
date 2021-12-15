using System.Threading.Tasks;
using UnityEngine;

public class MazeBoard : MonoBehaviour
{
    [SerializeField] QlValueAIPlayer aiPlayer;
    [SerializeField] private int HEIGHT = 18;
    [SerializeField] private int WIDTH = 30;

    private MazeNode[,] mazeNodes;
    
    private int numTargets;

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

                transform.position = GetPosition(row, col).Value;

                mazeNodes[row, col] = mazeNode.GetComponent<MazeNode>();
            }
        }

        numTargets = 0;

        SetIsChangeable(true);

        Invoke("doing", 30);
    }

    private void doing()
    {
        SetIsChangeable(false);
        aiPlayer.Play("Assets/Scripts/dir2");
        Algorithm alg = (new GameObject("Algorithm")).AddComponent<DFS>();
        Debug.Log(alg.IsSolvable(0, 0, mazeNodes, getNumTargets()));
    }

    public int getNumTargets()
    {
        return numTargets;
    }

    public void SetIsChangeable(bool b)
    {
        MazeNode.SetIsChangeable(b);

        if (!b)
        {
            numTargets = 0;
            foreach (var mazeNode in mazeNodes) {
                if(mazeNode.State == MazeNode.States.TARGERT)
                {
                    ++numTargets;
                }
            }
        }
    }

    public void intialzeTargets()
    {
        foreach(var mazeNode in mazeNodes)
        {
            if(mazeNode.State == MazeNode.States.TARGET_REACHED)
            {
                mazeNode.State = MazeNode.States.TARGERT;
            }
        }
    }

    public Vector3? GetPosition(int row, int col)
    {
        if(row >= HEIGHT || row < 0 || col < 0 || col >=WIDTH)
        {
            return null;
        }

        return new Vector3((((float)-WIDTH) / 2 + col + 0.5f) * MazeNode.WIDTH, ((float)HEIGHT / 2 - row - 0.5f) * MazeNode.HEIGHT);
    }

    public MazeNode getMazeNode(int row, int col)
    {
        return mazeNodes[row, col];
    }

    public override string ToString() {
        string strMaze = "";

        for (int row = 0; row < HEIGHT; ++row)
        {
            for (int col = 0; col < WIDTH; ++col)
            {
                if(col != 0)
                {
                    strMaze += ' ';
                }

                switch(mazeNodes[row, col].State)
                {
                    case MazeNode.States.BLOCKED:
                        strMaze += 'b';
                        break;

                    case MazeNode.States.UNBLOCKED:
                        strMaze += 'e';
                        break;

                    case MazeNode.States.TARGERT:
                        strMaze += 't';
                        break;
                    default:
                        throw new System.Exception($"got un non state in maze node {row}x{col}");
                }
            }

            strMaze += '\n';
        }

        return strMaze;
    }
}
