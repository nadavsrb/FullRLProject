using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] AlgorithmObject algObj;
    [SerializeField] TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (text.text == "Start")
            {
                if (!player.MazeBoard.GetIsChangeable()) return;

                algObj.StartAlgSearch(player);

                text.text = "Stop";
            } else
            {
                if (player.MazeBoard.GetIsChangeable()) return;

                algObj.StopAlgSearch();

                text.text = "Start";
            }
        });

        algObj.StopLisners += delegate ()
        {
            text.text = "Start";
        };
    }
}
