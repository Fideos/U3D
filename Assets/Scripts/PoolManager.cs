using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static private PoolManager instance;

    static public PoolManager Instance()
    {
        return instance;
    }

    [SerializeField]
    ObjectPool[] poolManager;

    public void ShootBullet(string name, Vector3 target, float speed, int damage, bool isEnemy)
    {
        for (int i = 0; i < poolManager.Length; i++)
        {
            if (poolManager[i].name == name)
            {
                poolManager[i].ShootBullet(target, speed, damage, isEnemy);
            }
        }
    }

    public void UpdatePosition(string name, Transform posToSet)
    {
        for (int i = 0; i < poolManager.Length; i++)
        {
            if (poolManager[i].name == name)
            {
                poolManager[i].pos = posToSet.transform.position;
            }
        }
    }

    public void DestroyObject(string name)
    {
        for (int i = 0; i < poolManager.Length; i++)
        {
            if (poolManager[i].name == name)
            {
                poolManager[i].PoolReturn();
            }
        }
    }

    public void CreateObject(string name)
    {
        for (int i = 0; i < poolManager.Length; i++)
        {
            if (poolManager[i].name == name)
            {
                poolManager[i].PoolRequest(poolManager[i].pos, poolManager[i].rot, poolManager[i].scale);
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < poolManager.Length; i++)
        {
            poolManager[i].Initialize();
        }
    }
}
