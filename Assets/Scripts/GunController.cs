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
    private int bulletDamage;
    private int bulletAngle;
    private float bulletsLeft;

    private float reloadTimer;

    /*AUDIO*/
    private AudioSource source;
    [SerializeField]
    private float volLowRange;
    [SerializeField]
    private float volHighRange;

    /*UI*/

    public int GetCurrentWeaponID()
    {
        return currentWeapon.gunID;
    }

    public float GetCurrentWeaponMagSize()
    {
        return currentWeapon.magSize;
    }

    public float GetBulletsLeft()
    {
        return bulletsLeft;
    }

    /*ANIMATOR*/

    public bool GetReloading()
    {
        return reloading;
    }


    public void WeaponPickup(Gun weaponPick)
    {
        ready = false;
        currentWeapon = weaponPick;
        RefreshWeapon();
    }

    private void Reload()
    {
        reloading = false;
        reloadTimer = currentWeapon.reloadSpeed;
        bulletsLeft = currentWeapon.magSize;
        source.PlayOneShot(currentWeapon.finishReloadingSound, volHighRange);
        Debug.Log("Reloaded");
    }

    private void Aim()
    {
        bulletDamage = Random.Range(currentWeapon.damage[0], currentWeapon.damage[1]);
        bulletAngle = Random.Range(-currentWeapon.recoil, currentWeapon.recoil);
        aim = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        aim = Camera.main.ScreenToWorldPoint(aim);
        this.gameObject.transform.LookAt(aim);
        this.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + bulletAngle, 0);
        //Debug.Log(aim);
        PoolManager.Instance().ShootBullet("Bullets", this.gameObject.transform.forward, currentWeapon.bulletSpeed, bulletDamage, false);
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
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(currentWeapon.shootSound, vol);
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
            source.PlayOneShot(currentWeapon.finishReloadingSound, volHighRange);
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
        source = GetComponent<AudioSource>();
        RefreshWeapon();
    }

    private void Update()
    {
        if (Input.GetKey(shootingBind) && ready && !reloading)
        {
            if (bulletsLeft > 0)
            {
                FireBullet();
            }
            else
            {
                if (Time.time > currentWeapon.fireRate + lastShot)
                {
                    source.PlayOneShot(currentWeapon.dryFireSound, volHighRange);
                    Debug.Log("Out of bullets.");
                    lastShot = Time.time;
                }
            }
        }

        if (Input.GetKey(reloadBind) && bulletsLeft < currentWeapon.magSize && reloading == false)
        {
            reloading = true;
            source.PlayOneShot(currentWeapon.startReloadingSound, volHighRange);
            Debug.Log("Reloading...");
        }
        if (reloading)
        {
            reloadTimer -= Time.deltaTime;
            if (Time.time > 1 + lastShot)
            {
                source.PlayOneShot(currentWeapon.midReloadingSound, volHighRange);
                lastShot = Time.time;
            }
        }
        if(reloadTimer <= 0)
        {
            Reload();
        }
    }
}
