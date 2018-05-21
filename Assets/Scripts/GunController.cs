using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField]
    private Gun currentWeapon;
    [SerializeField]
    private KeyCode shootingBind;
    [SerializeField]
    private KeyCode reloadBind;
    [SerializeField]
    private float distance;

    private float lastShot = 0.0f;
    private Vector3 aim;
    private bool ready;
    private bool reloading;
    private int bulletAngle;
    private float bulletsLeft;

    private float reloadTimer;

    private void Reload()
    {
        reloading = false;
        reloadTimer = currentWeapon.reloadSpeed;
        bulletsLeft = currentWeapon.magSize;
        Debug.Log("Reloaded");
    }

    private void Aim()
    {
        bulletAngle = Random.Range(-currentWeapon.recoil, currentWeapon.recoil);
        aim = new Vector3(Input.mousePosition.x + bulletAngle, Input.mousePosition.y, distance);
        aim = Camera.main.ScreenToWorldPoint(aim);
        this.gameObject.transform.LookAt(aim);
        this.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        //Debug.Log(aim);
        PoolManager.Instance().ApplyForce("Bullets", this.gameObject.transform.forward, currentWeapon.bulletSpeed);
    }

    private void FireBullet()
    {
        if (Time.time > currentWeapon.fireRate + lastShot)
        {
            for(int i = 0; i < currentWeapon.bulletsPerShot; i++)
            {
                PoolManager.Instance().UpdatePosition("Bullets", this.transform);
                PoolManager.Instance().CreateObject("Bullets");
                Aim();
            }
            lastShot = Time.time;
            bulletsLeft--;
        }
    }

    private void RefreshWeapon()
    {
        reloading = false;
        if (currentWeapon != null)
        {
            ready = true;
            bulletsLeft = currentWeapon.magSize;
            Debug.Log(currentWeapon.name + " ready!");
        }
        else
        {
            ready = false;
            Debug.Log("You don't have a weapon.");
        }
        reloadTimer = currentWeapon.reloadSpeed;
    }

    private void Awake()
    {
        RefreshWeapon();
    }

    private void Update()
    {
        if (Input.GetKey(shootingBind) && ready)
        {
            if (bulletsLeft > 0)
            {
                FireBullet();
            }
            else
            {
                Debug.Log("Out of bullets.");
            }
        }

        if (Input.GetKey(reloadBind) && bulletsLeft < currentWeapon.magSize && reloading == false)
        {
            reloading = true;
            Debug.Log("Reloading...");
        }
        if (reloading)
        {
            reloadTimer -= Time.deltaTime;
        }
        if(reloadTimer <= 0)
        {
            Reload();
        }
    }
}
