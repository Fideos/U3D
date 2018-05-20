using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private bool destroyOnCollision;



    private void DestroyBullet()
    {
        this.gameObject.transform.position = new Vector3(0, 0, 0);
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
        //Debug.Log("Bullet Destroyed");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided");
        if (destroyOnCollision)
        {
            DestroyBullet();
        }
    }
}
