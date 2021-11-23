using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform transform;
    [SerializeField] protected MazeBoard mazeBoard;

    protected bool isMoveEnded = true;
    protected Vector2Int posInMaze; // X, Y in the maze grid

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();

        transform.localScale = new Vector3(MazeNode.WIDTH, MazeNode.HEIGHT);

        SetPosition(0, 0);
    }

    public void SetPosition(int row, int col, bool isAnimated=false)
    {
        var newPos = mazeBoard.GetPosition(row, col);
        if (newPos == null) return;//invalid pos
        if (mazeBoard.getMazeNode(row, col).State == MazeNode.States.BLOCKED) return;
  
        posInMaze.x = row;
        posInMaze.y = col;

        if (isAnimated)
        {
            isMoveEnded = false;
            StartCoroutine(Move(newPos.Value, 5));
        }
        else
        {
            transform.position = newPos.Value;

            if (mazeBoard.getMazeNode(row, col).State == MazeNode.States.TARGERT)
            {
                mazeBoard.getMazeNode(row, col).State = MazeNode.States.TARGET_REACHED;
            }
        }
    }


    private IEnumerator Move(Vector3 newPos, int frames)
    {
        var wait = new WaitForSeconds(0.03f);

        var oldPos = transform.position;

        for(int i=1; i < frames; ++i)
        {
            transform.position = (oldPos - newPos) * (1 - (i / (float)frames)) + newPos;
            yield return wait;
        }

        transform.position = newPos;

        if (mazeBoard.getMazeNode(posInMaze.x, posInMaze.y).State == MazeNode.States.TARGERT)
        {
            mazeBoard.getMazeNode(posInMaze.x, posInMaze.y).State = MazeNode.States.TARGET_REACHED;
        }

        isMoveEnded = true;
    }

        protected void MoveUp()
    {
        SetPosition(posInMaze.x - 1, posInMaze.y, true);
    }
    protected void MoveDown()
    {
        SetPosition(posInMaze.x + 1, posInMaze.y, true);
    }
    protected void MoveRight()
    {
        SetPosition(posInMaze.x, posInMaze.y + 1, true);
    }
    protected void MoveLeft()
    {
        SetPosition(posInMaze.x, posInMaze.y - 1, true);
    }
}
