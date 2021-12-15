using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransformLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadSceneByName(string SceneName)
    {
        StartCoroutine(LoadScene(SceneName));
    }

    private IEnumerator LoadScene(string SceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(SceneName);
    }
}
