using System.Threading.Tasks;
using UnityEngine;

public class MazeBoard : MonoBehaviour
{
    [SerializeField] QlValueAIPlayer aiPlayer;
    [SerializeField] RectTransform env;
    [SerializeField] private int numNodesWidth;
    private int numNodesHeight;

    private MazeNode[,] mazeNodes;

    private int numTargets;

    [SerializeField] GameObject prefabMazeNode;

    private bool isIntialized = false;
    public bool IsIntialized{
        get => isIntialized;
    }

    public int NumNodesWidth
    {
        get => numNodesWidth;
    }

    public int NumNodesHeight
    {
        get => numNodesHeight;
    }

    private bool isChangeble = false;

    // Start is called before the first frame update
    void Start()
    {
        var dimNode = env.rect.width / numNodesWidth;
        numNodesHeight = (int) (env.rect.height / dimNode);
        MazeNode.SetSize(dimNode);

        mazeNodes = new MazeNode[numNodesHeight, numNodesWidth];

        Transform transform;

        for (int row = 0; row < numNodesHeight; ++row)
        {
            for(int col = 0; col < numNodesWidth; ++col)
            {
                var mazeNode = Instantiate(prefabMazeNode);

                transform = mazeNode.GetComponent<Transform>();

                transform.SetParent(GetComponent<Transform>());

                transform.localPosition = GetPosition(row, col).Value;

                mazeNodes[row, col] = mazeNode.GetComponent<MazeNode>();
            }
        }

        numTargets = 0;

        SetIsChangeable(true);

        isIntialized = true;
    }

    private void doing() //////////////////
    {
        aiPlayer.Play("Assets/Scripts/dir2");
    }

    public int GetNumTargets()
    {
        return numTargets;
    }

    public void SetIsChangeable(bool b)
    {
        isChangeble = b;

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

    public bool GetIsChangeable()
    {
        return isChangeble;
    }

    public void IntialzeTargets()
    {
        foreach(var mazeNode in mazeNodes)
        {
            if(mazeNode.State == MazeNode.States.TARGET_REACHED)
            {
                mazeNode.State = MazeNode.States.TARGERT;
            }
        }
    }

    public void ResetNodes()
    {
        foreach (var mazeNode in mazeNodes)
        {
            mazeNode.State = MazeNode.States.UNBLOCKED;
        }
    }

    public Vector3? GetPosition(int row, int col)
    {
        if(row >= numNodesHeight || row < 0 || col < 0 || col >= numNodesWidth)
        {
            return null;
        }

        return new Vector3((((float)-numNodesWidth) / 2 + col + 0.5f) * MazeNode.WIDTH,
            ((float)numNodesHeight / 2 - row - 0.5f) * MazeNode.HEIGHT);
    }

    public MazeNode GetMazeNode(int row, int col)
    {
        return mazeNodes[row, col];
    }
    public MazeNode[,] GetMazeNodes()
    {
        return mazeNodes;
    }

    public override string ToString() {
        string strMaze = "";

        for (int row = 0; row < numNodesHeight; ++row)
        {
            for (int col = 0; col < numNodesWidth; ++col)
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
