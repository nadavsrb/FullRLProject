using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSceneTransition : MonoBehaviour
{
    public TransformLoader transformLoader;
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate()
        {
            transformLoader.LoadSceneByName(sceneToLoad);
        });
    }

}
