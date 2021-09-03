
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithm: MonoBehaviour
{
    protected enum AnimAction
    {
        MARKE,
        UNMARKE,
        UNMARKE_ALL
    }
    
    public static readonly float DELAY_SEC = 0.01f;
    public static readonly float SEC_BEFORE_END_ANIM = 1F;
    protected IList<Vector3Int> animSearchList;
    protected MazeNode targetNode = null;

    public bool IsSolvable(int rowPlayer, int colPlayer, MazeNode[,] mazeNode)
    {
        animSearchList = new List<Vector3Int>();
        targetNode = null;

        UnFatherAll(mazeNode);

        bool isSolvable = Solve(rowPlayer,colPlayer, mazeNode);

        UnMarkeAll(mazeNode);
        StartCoroutine(DisplayAlg(mazeNode));

        return isSolvable;
    }

    protected abstract bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode);

    private void UnMarkeAll(MazeNode[,] mazeNode)
    {
        foreach (Vector3Int vec in animSearchList)
        {
            mazeNode[vec.x, vec.y].MarkedState = false;
        }
    }

    private void UnFatherAll(MazeNode[,] mazeNode)
    {
        foreach (Vector3Int vec in animSearchList)
        {
            mazeNode[vec.x, vec.y].FatherNode = null;
        }
    }

    private IEnumerator DisplayAlg(MazeNode[,] mazeNode)
    {
        var wait = new WaitForSeconds(DELAY_SEC);

        MazeNode.SetShouldMarkeWithColor(true);

        foreach (Vector3Int vec in animSearchList)
        {
            doAnimAction(vec, mazeNode);

            yield return wait;
        }

        MazeNode.SetShouldMarkeWithSpecialColor(true);

        MazeNode node = targetNode;
        while (node != null)
        {
            node.MarkedState = true;
            node = node.FatherNode;

            yield return wait;
        }

        MazeNode.SetShouldMarkeWithSpecialColor(false);

        yield return new WaitForSeconds(SEC_BEFORE_END_ANIM);

        UnMarkeAll(mazeNode);
        UnFatherAll(mazeNode);

        MazeNode.SetShouldMarkeWithColor(false);
    }
    
    private void doAnimAction(Vector3Int vec, MazeNode[,] mazeNode)
    {
        switch ((AnimAction)vec.z)
        {
            case AnimAction.MARKE:
                mazeNode[vec.x, vec.y].MarkedState = true;
                break;
            case AnimAction.UNMARKE:
                mazeNode[vec.x, vec.y].MarkedState = false;
                break;
            case AnimAction.UNMARKE_ALL:
                UnMarkeAll(mazeNode);
                break;
            default:
                Debug.Log("Error: An Unknown animation action");
                break;
        }
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
        return SolveHelper(rowPlayer, colPlayer, mazeNode, null);
    }

    private bool SolveHelper(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, MazeNode fatherNode)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
            mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED ||
            mazeNode[rowPlayer, colPlayer].MarkedState == true) return false;

        mazeNode[rowPlayer, colPlayer].FatherNode = fatherNode;
        mazeNode[rowPlayer, colPlayer].MarkedState = true;
        animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.MARKE));

        if (mazeNode[rowPlayer, colPlayer].State == MazeNode.States.TARGERT)
        {
            targetNode = mazeNode[rowPlayer, colPlayer];
            return true;
        }

        bool isSolvable = SolveHelper(rowPlayer, colPlayer + 1, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer - 1, colPlayer, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer + 1, colPlayer, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer, colPlayer - 1, mazeNode, mazeNode[rowPlayer, colPlayer]);

        // animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.UNMARKE));

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
        animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.MARKE));

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

                mazeNode[newPos.x, newPos.y].FatherNode = mazeNode[currPos.x, currPos.y];
                mazeNode[newPos.x, newPos.y].MarkedState = true;
                animSearchList.Add(new Vector3Int(newPos.x, newPos.y, (int)AnimAction.MARKE));

                if(mazeNode[newPos.x, newPos.y].State == MazeNode.States.TARGERT)
                {
                    targetNode = mazeNode[newPos.x, newPos.y];

                    while (queue.Count != 0)
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