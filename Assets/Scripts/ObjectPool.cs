using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    public string name;
    public GameObject objectToPool;
    public int poolSize;
    public Vector3 pos, scale, rot;
    public bool expand;

    private int lastSpawned;

    [SerializeField]
    GameObject[] pool;

    public void ApplyForce(Vector3 target, float speed)
    {
        Rigidbody rb;
        rb = pool[lastSpawned].GetComponent<Rigidbody>();
        rb.AddForce(target * speed);
    }

    GameObject[] ForceExpandRequest(GameObject[] pool)
    {
        GameObject[] expandedPool = new GameObject[pool.Length + 1];
        for (int i = 0; i < pool.Length; i++)
        {
            expandedPool[i] = pool[i];
        }
        GameObject obj = (GameObject)GameObject.Instantiate(objectToPool);
        obj.SetActive(false);
        expandedPool[expandedPool.Length-1] = obj;
        return expandedPool;
    }

    public void PoolReturn()
    {
        int i = 0;
        GameObject obj;
        obj = pool[i];
        while (!obj.activeInHierarchy && i < pool.Length - 1)
        {
            i++;
            obj = pool[i];
        }
        obj.SetActive(false);
    }

    public void PoolRequest(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        int i = 0;
        GameObject obj;
        obj = pool[i];
        while (obj.activeInHierarchy && i <= pool.Length - 1)
        {
            i++;
            if (i < pool.Length)
            {
                obj = pool[i];
            }
        }
        if (i == pool.Length && expand)
        {
            pool = ForceExpandRequest(pool);
            obj = pool[i];
        }
        obj.transform.position = position;
        obj.transform.Rotate(rot);
        obj.transform.localScale = scale;
        obj.SetActive(true);
        lastSpawned = i;
    }

    public void Initialize()
    {
        pool = new GameObject[poolSize];
        if (scale == new Vector3(0, 0, 0))
        {
            scale = objectToPool.transform.localScale;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(objectToPool);
            obj.SetActive(false);
            pool[i] = obj;
        }
    }
}
