using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    [SerializeField]
    private AIController ai;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private Gun weapon;
    [SerializeField]
    private float rotationSpeed;

    private float hpUpdate;
    private CharacterController controller;

    private Vector3 vectorDir;

    public void Rotate(Transform target)
    {
        this.gameObject.transform.LookAt(target);
    }

    public bool getHit()
    {
        if(hpUpdate != hp)
        {
            hpUpdate = hp;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool hasWeapon()
    {
        if(weapon != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetHP()
    {
        return hp;
    }

    public void Movement(Transform target, float speed)
    {
        vectorDir = target.position - transform.position;
        vectorDir *= speed;
        if (!controller.isGrounded)
        {
            vectorDir.y -= gravity * Time.deltaTime;
        }
        controller.Move(vectorDir * Time.deltaTime);
        Rotate(target);
        this.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    public void Die()
    {
        Destroy(this.gameObject.GetComponent<CharacterController>());
    }

    public void DealDamage(float damage)
    {
        hp = hp - damage;
    }

    private void Awake()
    {
        hpUpdate = hp;
        controller = this.gameObject.GetComponent<CharacterController>();
    }

}
