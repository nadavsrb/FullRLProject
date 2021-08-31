
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithm: MonoBehaviour
{
    public static readonly int MARKE = 1;
    public static readonly int UNMARKE = 0;
    public static readonly float DELAY_SEC = 0.02f;
    public IList<Vector3Int> animationList;

    public bool IsSolvable(int rowPlayer, int colPlayer, MazeNode[,] mazeNode)
    {
        animationList = new List<Vector3Int>();

        bool isSolvable = Solve(rowPlayer,colPlayer, mazeNode);

        UnMarkeAll(mazeNode);
        StartCoroutine(DisplayAlg(mazeNode));

        return isSolvable;
    }

    protected abstract bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode);

    private void UnMarkeAll(MazeNode[,] mazeNode)
    {
        foreach (Vector3Int vec in animationList)
        {
            mazeNode[vec.x, vec.y].MarkedState = false;
        }
    }

    private IEnumerator DisplayAlg(MazeNode[,] mazeNode)
    {
        var wait = new WaitForSeconds(DELAY_SEC);

        MazeNode.SetShouldMarkeWithColor(true);

        foreach (Vector3Int vec in animationList)
        {
            mazeNode[vec.x, vec.y].MarkedState = (vec.z == MARKE);
            yield return wait;
        }

        UnMarkeAll(mazeNode);

        MazeNode.SetShouldMarkeWithColor(false);
    } 

    protected bool IsValid(int rowPlayer, int colPlayer, MazeNode[,] mazeNode)
    {
        int numRows = mazeNode.GetLength(0);
        int numCols = mazeNode.GetLength(1);
        return (0 <= rowPlayer && rowPlayer < numRows) &&
            (0 <= colPlayer && colPlayer < numCols);
    }
}


public class DFS : Algorithm
{
    protected override bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
            mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED ||
            mazeNode[rowPlayer, colPlayer].MarkedState == true) return false;

        
        mazeNode[rowPlayer, colPlayer].MarkedState = true;
        animationList.Add(new Vector3Int(rowPlayer, colPlayer, MARKE));

        if (mazeNode[rowPlayer, colPlayer].State == MazeNode.States.TARGERT)
        {
            animationList.Add(new Vector3Int(rowPlayer, colPlayer, UNMARKE));
            return true;
        }
        
        bool isSolvable = Solve(rowPlayer, colPlayer + 1, mazeNode) ||
             Solve(rowPlayer - 1, colPlayer, mazeNode) ||
             Solve(rowPlayer + 1, colPlayer, mazeNode) ||
             Solve(rowPlayer, colPlayer - 1, mazeNode);

        animationList.Add(new Vector3Int(rowPlayer, colPlayer, UNMARKE));
        return isSolvable;
    }
}

public class BFS : Algorithm
{
    protected override bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
           mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED) return false;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(rowPlayer, colPlayer));

        mazeNode[rowPlayer, colPlayer].MarkedState = true;
        animationList.Add(new Vector3Int(rowPlayer, colPlayer, MARKE));

        bool isSolvable = false;
        while(queue.Count != 0)
        {
            var currPos = queue.Dequeue();

            Vector2Int[] newPoss = {
                new Vector2Int(currPos.x , currPos.y + 1),
                new Vector2Int(currPos.x - 1, currPos.y),
                new Vector2Int(currPos.x , currPos.y - 1),
                new Vector2Int(currPos.x + 1, currPos.y) 
            };

            foreach (var newPos in newPoss)
            {
                if (!IsValid(newPos.x, newPos.y, mazeNode) ||
                    mazeNode[newPos.x, newPos.y].State == MazeNode.States.BLOCKED ||
                     mazeNode[newPos.x, newPos.y].MarkedState == true) continue;

                mazeNode[newPos.x, newPos.y].MarkedState = true;
                animationList.Add(new Vector3Int(newPos.x, newPos.y, MARKE));

                if(mazeNode[newPos.x, newPos.y].State == MazeNode.States.TARGERT)
                {
                    while(queue.Count != 0)
                    {
                        var oldPos = queue.Dequeue();
                    }

                    isSolvable = true;

                    break;
                }

                queue.Enqueue(newPos);
            }
        }

        return isSolvable;
    }
}