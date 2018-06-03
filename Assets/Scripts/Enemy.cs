using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    [SerializeField]
    private AIController ai;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private Gun weapon;

    private float hpUpdate;
    private CharacterController controller;

    private Vector3 vectorDir;

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

    private bool IsGrounded()
    {
        bool answer = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        return answer;
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

    public void Movement(Transform target)
    {
        vectorDir = target.position - transform.position;
        vectorDir *= speed;
        controller.Move(vectorDir * Time.deltaTime);
        this.gameObject.transform.LookAt(target);
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
