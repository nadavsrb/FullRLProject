using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MazeBoard : MonoBehaviour
{
    public static readonly int HEIGHT = 12;
    public static readonly int WIDTH = 20;

    private InnerMazeNode[,] mazeNodes;
    private int[,] mazeBoard;

    [SerializeField] GameObject prefabMazeNode;

    private class InnerMazeNode
    {
        private MazeNode mazeNode;
        
        public InnerMazeNode(int row, int col, MazeNode mazeNode, int[,] mazeBoard)
        {
            this.mazeNode = mazeNode;

            mazeNode.notify += delegate
            {
                mazeBoard[row, col] = (mazeBoard[row, col] + 1) % 3;
            };
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mazeNodes = new InnerMazeNode[HEIGHT,WIDTH];
        mazeBoard = new int[HEIGHT, WIDTH];

        for (int row = 0; row < HEIGHT; ++row)
        {
            for(int col = 0; col < WIDTH; ++col)
            {
                mazeBoard[row, col] = 0;

                var mazeNode = Instantiate(prefabMazeNode);

                var transform = mazeNode.GetComponent<Transform>();

                transform.SetParent(GetComponent<Transform>());

                transform.position = getPosition(row, col);

                mazeNodes[row, col] = new InnerMazeNode(row, col, mazeNode.GetComponent<MazeNode>(), mazeBoard);
            }
        }

        setIsChangeable(true);
    }

    public void setIsChangeable(bool b)
    {
        MazeNode.setIsChangeable(b);
    }

    public static Vector3 getPosition(int row, int col)
    {
        return new Vector3((((float)-WIDTH) / 2 + col + 0.5f) * MazeNode.WIDTH, ((float)HEIGHT / 2 - row - 0.5f) * MazeNode.HEIGHT);
    }

}
