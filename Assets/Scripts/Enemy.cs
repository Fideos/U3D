using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    [SerializeField]
    private AIController controller;
    [SerializeField]
    private float hp;

    public float GetHP()
    {
        return hp;
    }

    public void Die()
    {
        Destroy(this.gameObject.GetComponent<Collider>());
    }

    public void DealDamage(float damage)
    {
        hp = hp - damage;
    }

}
