using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {



    [SerializeField]
    private bool destroyOnCollision;
    [SerializeField]
    private int bulletDamage;

    private bool hit;

    public void SetBulletDamage(int setDamage)
    {
        bulletDamage = setDamage;
    }

    private void DestroyBullet()
    {
        this.gameObject.transform.position = new Vector3(0, 0, 0);
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
        hit = false;
        //Debug.Log("Bullet Destroyed");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!hit) // Reformular mas tarde
        {
            if (other.gameObject.tag == "Enemy")
            {
                Enemy target = other.gameObject.GetComponent<Enemy>();
                target.RecieveDamage(bulletDamage);
                hit = true;
            }
        }
        if (destroyOnCollision)
        {
            DestroyBullet();
        }
    }
}
