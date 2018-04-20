using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolControllerTest : MonoBehaviour {

    public KeyCode cubeSpawnBind;
    public KeyCode sphereSpawnBind;

    public KeyCode destroyCubeBind;
    public KeyCode destroySphereBind;

    private void Update()
    {
        //Llamados por Nombre
        if (Input.GetKeyDown(cubeSpawnBind))
        {
           PoolManager.Instance().CreateObject("Cubos");
        }
        if (Input.GetKeyDown(sphereSpawnBind))
        {
            PoolManager.Instance().CreateObject("Spheres");
        }
        if (Input.GetKeyDown(destroyCubeBind))
        {
            PoolManager.Instance().DestroyObject("Cubos");
        }
        if (Input.GetKeyDown(destroySphereBind))
        {
            PoolManager.Instance().DestroyObject("Spheres");
        }
    }
}
