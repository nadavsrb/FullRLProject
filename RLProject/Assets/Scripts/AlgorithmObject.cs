using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmObject : MonoBehaviour
{
    public delegate void StopLisner();
    public event StopLisner StopLisners;

    public delegate void AlgChangedLisner();
    public event StopLisner AlgChangedLisners;

    private string algName = "BFS";
    public string AlgName
    {
        get => algName;
  
    }

    private Algorithm alg;

    void Start()
    {
        gameObject.AddComponent<DFS>().EndAnimLisners += OnStopAnim;
        gameObject.AddComponent<BFS>().EndAnimLisners += OnStopAnim;

        alg = GetComponent<BFS>();
    }

    private void OnStopAnim()
    {
        StopLisners();
    }

    public void StartAlgSearch(Player player)
    {
        alg.IsSolvable(player);
    }

    public void StopAlgSearch()
    {
        alg.StopAnim();
    }

    public void NextAlg()
    {
        if (algName == "BFS")
        {
            algName = "DFS";
            alg = GetComponent<DFS>();
        }
        else
        {
            algName = "BFS";
            alg = GetComponent<BFS>();
        }

        AlgChangedLisners();
    }
}
