using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int gunID;
    public string gunName;
    public int[] damage;
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

    private bool playerInTrigger;

    private Player player;
    
    [SerializeField]
    private bool destroyOnPickup;

    private MyGameManager currentManager;

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
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
    */

    private void OnTriggerEnter(Collider other) //Lo cambié por que provocaba un drop de frames.
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Ready to pick");
            player = other.GetComponent<Player>();
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Not ready to pick");
            playerInTrigger = false;
        }
    }

    private void Awake()
    {
        /*
        if (damage != new int[2])
        {
            damage = new int[2];
            damage[0] = 30;
            damage[1] = 50;
        }
        */
        //Debug.Log(damage[0] + ", " + damage[1]);

        playerInTrigger = false;

        if(bulletsPerShot <= 0)
        {
            bulletsPerShot = 1;
        }
        if(name == null)
        {
            name = "Default";
        }
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
        currentManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MyGameManager>();
    }

    private void Update()
    {
        if (playerInTrigger)
        {
            currentManager.PickupPop(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.handReference.WeaponPickup(this);
                if (destroyOnPickup)
                {
                    currentManager.PickupPop(false);
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            currentManager.PickupPop(false);
        }
    }

}
