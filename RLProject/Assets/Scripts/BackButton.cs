using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public Animator anim;
    public float animTime = 0.5f;
    public TransformLoader transformLoader;
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            StartCoroutine(BackAnimation());
        });
    }

    private IEnumerator BackAnimation()
    {
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(animTime);

        transformLoader.LoadSceneByName(sceneToLoad);
    }
}
