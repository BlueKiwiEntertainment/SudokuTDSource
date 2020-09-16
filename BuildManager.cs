using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject Player1Prefab;

    public static BuildManager instance;

    private void Awake()
    {
        BuildManager.instance = this;
    }



}
