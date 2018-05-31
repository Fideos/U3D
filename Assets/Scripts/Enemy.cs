using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private float hp;

    private void Die()
    {
        Destroy(this.gameObject.GetComponent<Collider>());
    }

    public void DealDamage(float damage)
    {
        hp = hp - damage;
    }

}
