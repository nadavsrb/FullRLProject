using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform transform;
    [SerializeField] protected MazeBoard mazeBoard;

    protected Vector2Int posInMaze; // X, Y in the maze grid

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();

        transform.localScale = new Vector3(MazeNode.WIDTH, MazeNode.HEIGHT);
    }

    public void SetPosition(int row, int col)
    {
        var newPos = mazeBoard.GetPosition(row, col);
        if (newPos == null) return;//invalid pos
        if (mazeBoard.getMazeNode(row, col).State == MazeNode.States.BLOCKED) return;
  
        posInMaze.x = row;
        posInMaze.y = col;

        transform.position = newPos.Value;

        if (mazeBoard.getMazeNode(row, col).State == MazeNode.States.TARGERT)
        {
            mazeBoard.getMazeNode(row, col).State = MazeNode.States.TARGET_REACHED;
        }
    }

    protected void MoveUp()
    {
        SetPosition(posInMaze.x - 1, posInMaze.y);
    }
    protected void MoveDown()
    {
        SetPosition(posInMaze.x + 1, posInMaze.y);
    }
    protected void MoveRight()
    {
        SetPosition(posInMaze.x, posInMaze.y + 1);
    }
    protected void MoveLeft()
    {
        SetPosition(posInMaze.x, posInMaze.y - 1);
    }
}
