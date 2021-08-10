using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int HEIGHT = 0;
    public int WIDTH = 0;
    

    private InnerMazeNode[,] mazeNodes;

    [SerializeField] GameObject prefabMazeNode;

    private class InnerMazeNode
    {
        private Object mazeNode;
        private int row;
        private int col;

        InnerMazeNode(int row, int col, Object mazeNode)
        {
            this.row = row;
            this.col = col;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mazeNodes = new InnerMazeNode[HEIGHT,WIDTH];

        for(int row = 0; row < HEIGHT; ++row)
        {
            for(int col = 0; col < WIDTH; ++col)
            {
                var mazeNode = Instantiate(prefabMazeNode);

                var transform = mazeNode.GetComponent<Transform>();

                transform.SetParent(GetComponent<Transform>());

                transform.position = new Vector3(((float)-WIDTH) / 2 + col + 0.5f, (float)HEIGHT / 2 - row - 0.5f);
            }
        }

        MazeNode.setIsChangeable(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
