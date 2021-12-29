using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform transform;
    [SerializeField] protected MazeBoard mazeBoard;
    public MazeBoard MazeBoard
    {
        get => mazeBoard;
    }

    protected bool isMoveEnded = true;
    protected Vector2Int posInMaze; // X, Y in the maze grid
    public Vector2Int PosInMaze
    {
        get => posInMaze;
    }

    private bool isStarted = false;

    // Start is called before the first frame update
    void Update()
    {
        if (mazeBoard.IsIntialized && !isStarted)
        {

            transform = GetComponent<Transform>();

            transform.localScale = new Vector3(MazeNode.WIDTH, MazeNode.HEIGHT);

            posInMaze.x = -1;
            posInMaze.y = -1;

            SetPosition(mazeBoard.NumNodesHeight / 2, mazeBoard.NumNodesWidth / 2);

            isStarted = true;
        }
    }
    
    public void SetMazeBoard(MazeBoard mazeBoard)
    {
        this.mazeBoard = mazeBoard;
    }

    public void SetPosition(int row, int col, bool isAnimated=false)
    {
        var newPos = mazeBoard.GetPosition(row, col);
        if (newPos == null) return;//invalid pos
        if (mazeBoard.GetMazeNode(row, col).State == MazeNode.States.BLOCKED) return;

        if (isAnimated)
        {
            if (posInMaze.x != -1 && posInMaze.x != -1)
            {
                mazeBoard.GetMazeNode(posInMaze.x, posInMaze.y).IsPlayerPlace = false;
            }

            mazeBoard.GetMazeNode(row, col).IsPlayerPlace = true;

            posInMaze.x = row;
            posInMaze.y = col;

            isMoveEnded = false;
            StartCoroutine(Move(newPos.Value, 5));
        }
        else
        {
            transform.localPosition = newPos.Value;

            if (posInMaze.x != -1 && posInMaze.x != -1)
            {
                mazeBoard.GetMazeNode(posInMaze.x, posInMaze.y).IsPlayerPlace = false;
            }

            mazeBoard.GetMazeNode(row, col).IsPlayerPlace = true;

            if (mazeBoard.GetMazeNode(row, col).State == MazeNode.States.TARGERT)
            {
                mazeBoard.GetMazeNode(row, col).State = MazeNode.States.TARGET_REACHED;
            }

            posInMaze.x = row;
            posInMaze.y = col;
        }
    }


    private IEnumerator Move(Vector3 newPos, int frames)
    {
        var wait = new WaitForSeconds(0.03f);

        var oldPos = transform.localPosition;

        for(int i=1; i < frames; ++i)
        {
            transform.localPosition = (oldPos - newPos) * (1 - (i / (float)frames)) + newPos;
            yield return wait;
        }

        transform.localPosition = newPos;

        if (mazeBoard.GetMazeNode(posInMaze.x, posInMaze.y).State == MazeNode.States.TARGERT)
        {
            mazeBoard.GetMazeNode(posInMaze.x, posInMaze.y).State = MazeNode.States.TARGET_REACHED;
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
