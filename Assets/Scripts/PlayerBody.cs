using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour {

    [SerializeField]
    private Player playerRef;
    [SerializeField]
    private GunController gunRef;

    private Animator anim;
    private Vector3 sight;

    private void GetSight()
    {
        sight = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        sight = Camera.main.ScreenToWorldPoint(sight);
        this.gameObject.transform.LookAt(sight);
        this.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void AnimUpdate()
    {
        anim.SetBool("isRunning", playerRef.GetMovement());
        anim.SetBool("isReloading", gunRef.GetReloading());
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update () {
        AnimUpdate();
        GetSight();
    }
}
