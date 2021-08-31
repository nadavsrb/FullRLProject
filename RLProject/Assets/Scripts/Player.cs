using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform transform;
    [SerializeField] Object prefabMazeNode;

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();

        transform.localScale = new Vector3(MazeNode.WIDTH, MazeNode.HEIGHT);

        SetPosition(0, 0);
    }

    public void SetPosition(int row, int col)
    {
        transform.position = MazeBoard.GetPosition(row, col);
    }
}
