using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunController : MonoBehaviour {

    [SerializeField]
    private Gun currentWeapon;

    private float lastShot = 0.0f;
    private bool ready;
    private int bulletDamage;
    private int bulletAngle;
    private float bulletsLeft;

    /*AUDIO*/
    private AudioSource source;
    [SerializeField]
    private float volLowRange;
    [SerializeField]
    private float volHighRange;

    public bool isReady()
    {
        return ready;
    }

    public float GetBulletsLeft()
    {
        return bulletsLeft;
    }

    public void Reload()
    {
        bulletsLeft = currentWeapon.magSize;
        source.PlayOneShot(currentWeapon.finishReloadingSound, volHighRange);
        Debug.Log("Enemy weapon reloaded");
    }

    private void Aim(Vector3 target)
    {
        bulletDamage = Random.Range(currentWeapon.damage[0], currentWeapon.damage[1]);
        bulletAngle = Random.Range(-currentWeapon.recoil, currentWeapon.recoil);
        this.gameObject.transform.LookAt(target);
        this.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + bulletAngle, 0);
        PoolManager.Instance().ShootBullet("EnemyBullets", this.gameObject.transform.forward, currentWeapon.bulletSpeed, bulletDamage, true);
    }

    public void Fire(Vector3 target)
    {
        if (Time.time > currentWeapon.fireRate + lastShot && bulletsLeft > 0)
        {
            for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
            {
                PoolManager.Instance().UpdatePosition("EnemyBullets", this.transform);
                PoolManager.Instance().CreateObject("EnemyBullets");
                Aim(target);
            }
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(currentWeapon.shootSound, vol);
            lastShot = Time.time;
            bulletsLeft--;
        }
    }

    private void RefreshWeapon()
    {
        if (currentWeapon != null)
        {
            ready = true;
            bulletsLeft = currentWeapon.magSize;
        }
        else
        {
            ready = false;
            Debug.Log("Enemy doesn't have a weapon...");
        }
    }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        RefreshWeapon();
    }


}
