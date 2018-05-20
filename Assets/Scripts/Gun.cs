using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public string gunName;
    public float fireRate;
    public float speed;
    public float magSize;
    public int recoil;
    public float reloadSpeed;


    private void Awake()
    {
        if(name == null)
        {
            name = "Default";
        }
    }

}
