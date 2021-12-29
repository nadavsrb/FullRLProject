
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithm: MonoBehaviour
{
    protected enum AnimAction
    {
        MARKE,
        UNMARKE,
        TARGET_PATH_MARKE
    }
    
    public static readonly float DELAY_SEC = 0.05f;
    public static float speed = 1f;
    public static readonly float SEC_BEFORE_END_ANIM = 1F;
    protected IList<Vector3Int> animSearchList;
    private int numAnimationRuning = 0;
    private MazeBoard maze;

    public delegate void EndAnimLisner();
    public event EndAnimLisner EndAnimLisners;

    public bool IsSolvable(Player player)
    {
        var pos = player.PosInMaze;
        var mazeBoard = maze = player.MazeBoard;
        int rowPlayer = pos.x;
        int colPlayer = pos.y;

        mazeBoard.SetIsChangeable(false);

        MazeNode[,] mazeNode = mazeBoard.GetMazeNodes();
        int numTargets = mazeBoard.GetNumTargets();

        animSearchList = new List<Vector3Int>();

        UnFatherAll(mazeNode);

        bool isSolvable = Solve(rowPlayer,colPlayer, mazeNode, numTargets);

        UnVisitAll(mazeNode);
        numAnimationRuning = 0;
        StartCoroutine(DisplayAlg(mazeNode, mazeBoard));

        return isSolvable;
    }

    protected abstract bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, int numTargets);

    public void StopAnim()
    {
        StopAllCoroutines();
        UnMarkeAll(maze.GetMazeNodes());
        UnFatherAll(maze.GetMazeNodes());

        maze.SetIsChangeable(true);
    }

    private void UnMarkeAll(MazeNode[,] mazeNode)
    {
        foreach (var node in mazeNode)
        {
            node.MarkedState = MazeNode.MarkType.UNMARKED;
        }
    }

    private void UnFatherAll(MazeNode[,] mazeNode)
    {
        foreach (var node in mazeNode)
        {
            node.FatherNode = null;
        }
    }

    private void UnVisitAll(MazeNode[,] mazeNode)
    {
        foreach (var node in mazeNode)
        {
            node.IsVisited = false;
        }
    }

    private IEnumerator DisplayAlg(MazeNode[,] mazeNode, MazeBoard mazeBoard)
    {
        var wait = new WaitForSeconds(DELAY_SEC / speed);

        foreach (Vector3Int vec in animSearchList)
        {
            ++numAnimationRuning;
            StartCoroutine(doAnimAction(vec, mazeNode));
            yield return wait;
        }

        yield return new WaitUntil(()=> numAnimationRuning == 0);
        yield return wait;

        UnMarkeAll(mazeNode);
        UnFatherAll(mazeNode);

        mazeBoard.SetIsChangeable(true);

        EndAnimLisners();
    }
    
    private IEnumerator doAnimAction(Vector3Int vec, MazeNode[,] mazeNode)
    {
        var wait = new WaitForSeconds(DELAY_SEC / speed);

        switch ((AnimAction)vec.z)
        {
            case AnimAction.TARGET_PATH_MARKE:
                MazeNode node = mazeNode[vec.x, vec.y];
                while (node != null)
                {
                    node.MarkedState = MazeNode.MarkType.TARGET_PATH_MARK;
                    node = node.FatherNode;

                    yield return wait;
                }
                break;
            case AnimAction.MARKE:
                mazeNode[vec.x, vec.y].MarkedState = MazeNode.MarkType.SCAN_MARK;
                break;
            case AnimAction.UNMARKE:
                mazeNode[vec.x, vec.y].MarkedState = MazeNode.MarkType.UNMARKED;
                break;
            default:
                Debug.Log("Error: An Unknown animation action");
                break;
        }

        --numAnimationRuning;
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
    private int numTargetsReached = 0;
    private int numTargets;

    protected override bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, int numTargets)
    {
        numTargetsReached = 0;
        this.numTargets = numTargets;
        return SolveHelper(rowPlayer, colPlayer, mazeNode, null);
    }

    private bool SolveHelper(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, MazeNode fatherNode)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
            mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED ||
            mazeNode[rowPlayer, colPlayer].IsVisited == true) return false;

        mazeNode[rowPlayer, colPlayer].FatherNode = fatherNode;
        mazeNode[rowPlayer, colPlayer].IsVisited = true;
        animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.MARKE));

        if (mazeNode[rowPlayer, colPlayer].State == MazeNode.States.TARGERT)
        {
            animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.TARGET_PATH_MARKE));
            ++numTargetsReached;
            
            if(numTargetsReached == numTargets) return true;
            
        }

        bool isSolvable = SolveHelper(rowPlayer, colPlayer + 1, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer - 1, colPlayer, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer + 1, colPlayer, mazeNode, mazeNode[rowPlayer, colPlayer]) ||
             SolveHelper(rowPlayer, colPlayer - 1, mazeNode, mazeNode[rowPlayer, colPlayer]);

        return isSolvable;
    }
}

public class BFS : Algorithm
{
    protected override bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, int numTargets)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
           mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED) return false;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(rowPlayer, colPlayer));

        mazeNode[rowPlayer, colPlayer].IsVisited = true;
        animSearchList.Add(new Vector3Int(rowPlayer, colPlayer, (int)AnimAction.MARKE));

        int numTargetsReached = 0;
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
                     mazeNode[newPos.x, newPos.y].IsVisited == true) continue;

                mazeNode[newPos.x, newPos.y].FatherNode = mazeNode[currPos.x, currPos.y];
                mazeNode[newPos.x, newPos.y].IsVisited = true;
                animSearchList.Add(new Vector3Int(newPos.x, newPos.y, (int)AnimAction.MARKE));

                if (mazeNode[newPos.x, newPos.y].State == MazeNode.States.TARGERT)
                {
                    animSearchList.Add(new Vector3Int(newPos.x, newPos.y, (int)AnimAction.TARGET_PATH_MARKE));
                    ++numTargetsReached;
                    if (numTargetsReached == numTargets) {
                        while (queue.Count != 0)
                        {
                            var oldPos = queue.Dequeue();
                        }

                        isSolvable = true;

                        break;
                    }
                }

                queue.Enqueue(newPos);
            }
        }

        return isSolvable;
    }
}

public class AStar : Algorithm
{
    private List<Vector2Int> targetPos;

    public class MyPriorityQueue
    {
        private List<Vector2Int> queue;
        private List<Vector2Int> targetPos;

        public int Count
        {
            get => queue.Count;
        }

        public MyPriorityQueue(List<Vector2Int> targetPos)
        {
            queue = new List<Vector2Int>();

            this.targetPos = targetPos;
        }

        public void Enqueue(Vector2Int vector2Int)
        {
            queue.Add(new Vector2Int(vector2Int.x, vector2Int.y));
        }

        public Vector2Int? Dequeue()
        {
            Vector2Int? elementToDequeue = null;
            double minDist = Double.PositiveInfinity;

            foreach(Vector2Int vec in queue)
            {
                double distCloseTarget = Double.PositiveInfinity;

                foreach(Vector2Int target in targetPos)
                {
                    double dist = Math.Sqrt(Math.Pow((vec.x - target.x), 2) + Math.Pow((vec.y - target.y), 2));

                    if (distCloseTarget > dist)
                    {
                        distCloseTarget = dist;
                    }
                }

                if(minDist == Double.PositiveInfinity || minDist > distCloseTarget)
                {
                    minDist = distCloseTarget;
                    elementToDequeue = new Vector2Int(vec.x, vec.y);
                }
            }

            if(elementToDequeue != null)
            {
                queue.RemoveAll(pos => (pos.x == elementToDequeue.Value.x) && (pos.y == elementToDequeue.Value.y));
            }

            return elementToDequeue;
        }
    }


    protected override bool Solve(int rowPlayer, int colPlayer, MazeNode[,] mazeNode, int numTargets)
    {
        if (!IsValid(rowPlayer, colPlayer, mazeNode) ||
           mazeNode[rowPlayer, colPlayer].State == MazeNode.States.BLOCKED) return false;

        targetPos = new List<Vector2Int>();

        for(int i = 0; i < mazeNode.GetLength(0); ++i)
        {
            for (int j = 0; j < mazeNode.GetLength(1); ++j)
            {
                if (mazeNode[i, j].State == MazeNode.States.TARGERT)
                {
                    targetPos.Add(new Vector2Int(i, j));
                }
            }
        }

        MyPriorityQueue queue = new MyPriorityQueue(targetPos);
        queue.Enqueue(new Vector2Int(rowPlayer, colPlayer));

        mazeNode[rowPlayer, colPlayer].IsVisited = true;

        int numTargetsReached = 0;
        bool isSolvable = false;
        while (queue.Count != 0)
        {
            var currPos = queue.Dequeue().Value;

            animSearchList.Add(new Vector3Int(currPos.x, currPos.y, (int)AnimAction.MARKE));

            if (mazeNode[currPos.x, currPos.y].State == MazeNode.States.TARGERT)
            {
                targetPos.RemoveAll(pos => (pos.x == currPos.x) && (pos.y == currPos.y));

                animSearchList.Add(new Vector3Int(currPos.x, currPos.y, (int)AnimAction.TARGET_PATH_MARKE));
                ++numTargetsReached;
                if (numTargetsReached == numTargets)
                {
                    while (queue.Count != 0)
                    {
                        var oldPos = queue.Dequeue().Value;
                    }

                    isSolvable = true;
                    break;
                }
            }

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
                     mazeNode[newPos.x, newPos.y].IsVisited == true) continue;

                mazeNode[newPos.x, newPos.y].FatherNode = mazeNode[currPos.x, currPos.y];
                mazeNode[newPos.x, newPos.y].IsVisited = true;

                queue.Enqueue(newPos);
            }
        }

        return isSolvable;
    }
}