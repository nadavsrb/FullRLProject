using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitOptionButton : MonoBehaviour
{
    [SerializeField] GameObject optionMenu;
    [SerializeField] MazeBoard mazeBoard;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            optionMenu.SetActive(false);
            mazeBoard.SetIsChangeable(true);
        });
    }
}
