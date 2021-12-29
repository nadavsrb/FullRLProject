using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlgButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AlgorithmObject algObj;
    void Start()
    {
        algObj.AlgChangedLisners += delegate ()
        {
            text.text = algObj.AlgName;
        };

        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            algObj.NextAlg();
        });
    }
}
