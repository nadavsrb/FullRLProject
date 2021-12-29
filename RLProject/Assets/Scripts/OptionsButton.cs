using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] GameObject optionMenu;
    [SerializeField] MazeBoard mazeBoard;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (!mazeBoard.GetIsChangeable()) return;

            optionMenu.SetActive(true);
            mazeBoard.SetIsChangeable(false);
        });
    }
}
