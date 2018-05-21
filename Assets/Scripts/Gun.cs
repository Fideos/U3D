using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public string gunName;
    public float fireRate;
    public float bulletSpeed;
    public int bulletsPerShot;
    public float magSize;
    public int recoil;
    public float reloadSpeed;


    private void Awake()
    {
        if(bulletsPerShot <= 0)
        {
            bulletsPerShot = 1;
        }
        if(name == null)
        {
            name = "Default";
        }
    }

}
