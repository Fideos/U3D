using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour {

    [SerializeField]
    private float hp;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float rotationSpeed;

    private float hpUpdate;

    private CharacterController controller;

    private Vector3 vectorDir;

    private MyGameManager currentGameManager;

    [SerializeField]
    private EnemyGunController handRef;

    [SerializeField]
    private AIShooter myAI;

    private void DropWeapon()
    {
        Instantiate(currentGameManager.GetWeaponDrops(handRef.currentWeapon.gunID), transform.position, Quaternion.identity);
    }

    public bool GunLoaded()
    {
        if (handRef.GetBulletsLeft() <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Rotate(Transform target)
    {
        this.gameObject.transform.LookAt(target);
    }

    public bool getHit()
    {
        if (hpUpdate != hp)
        {
            hpUpdate = hp;
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

    public void Die()
    {
        Destroy(this.gameObject.GetComponent<CharacterController>());
        Destroy(myAI);
        DropWeapon();
    }

    public void RecieveDamage(float damage)
    {
        hp = hp - damage;
    }

    public void Reload()
    {
        handRef.Reload();
    }

    public void Attack(Transform target)
    {
        handRef.Fire(target.position);
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

    private void Awake()
    {

        hpUpdate = hp;
        controller = this.gameObject.GetComponent<CharacterController>();
        currentGameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MyGameManager>();
    }

}
