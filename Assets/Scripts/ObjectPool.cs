using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly List<GameObject> pooledFastenerIcons = new();

    private readonly int amountToPool = 200;

    //assigned in scene:
    public GameObject objectToPool;
    public Transform poolParent; //read by Block

    private void Awake()
    {
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, poolParent);
            tmp.SetActive(false);
            //must populate pooledFasteners before Block calls GetPooledFastener at Start
            pooledFastenerIcons.Add(tmp);
        }
    }

    public GameObject GetPooledFastener()
    {
        foreach (GameObject fastenerIcon in pooledFastenerIcons)
        {
            if (!fastenerIcon.activeSelf)
                return fastenerIcon;
        }
        Debug.LogError("No available objects in pool");
        return default;
    }
}