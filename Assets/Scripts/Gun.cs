﻿using System.Collections;
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

    /*Audio*/
    public AudioClip shootSound;
    public AudioClip dryFireSound;
    public AudioClip startReloadingSound;
    public AudioClip midReloadingSound;
    public AudioClip finishReloadingSound;

    [SerializeField]
    private bool destroyOnPickup;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Ready to pick");
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.GetComponent<Player>().handReference.WeaponPickup(this);
                if (destroyOnPickup)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

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
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

}
