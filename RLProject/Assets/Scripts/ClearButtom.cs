using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearButtom : MonoBehaviour
{
    [SerializeField] MazeBoard mazeBoard;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (!mazeBoard.GetIsChangeable()) return;

            mazeBoard.ResetNodes();
        });
    }
}
