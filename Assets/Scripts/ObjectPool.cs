using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly List<GameObject> pooledFasteners = new();

    private readonly int amountToPool = 200;

    //assigned in scene:
    public GameObject objectToPool;
    public Transform poolParent;

    private void Start()
    {
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, poolParent);
            tmp.SetActive(false);
            pooledFasteners.Add(tmp);
        }
    }

    public GameObject GetPooledFastener()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledFasteners[i].activeSelf)
                return pooledFasteners[i];
        }
        Debug.LogError("No available objects in pool");
        return default;
    }
}