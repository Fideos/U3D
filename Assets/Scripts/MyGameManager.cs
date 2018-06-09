using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour {

    [SerializeField]
    private Player myPlayer;
    [SerializeField]
    private GunController playerHand;

    [SerializeField]
    private float hp;
    [SerializeField]
    private float bulletsLeft;

    public void RecieveDamage(float damage)
    {
        hp -= damage;
    }

    private void Awake()
    {
        if (myPlayer == null)
        {
            myPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        playerHand = myPlayer.handReference;
    }


    private void Update () {

        bulletsLeft = playerHand.GetBulletsLeft();
        if (hp <= 0)
        {
            myPlayer.Die();
        }
        
	}
}
