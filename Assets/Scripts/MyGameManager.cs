using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour {



    [SerializeField]
    private Player myPlayer;
    [SerializeField]
    private GunController playerHand;
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Image currentWeapon;
    [SerializeField]
    private Text bulletsMarker;

    private float maxHealth;
    private float currentHealth;

    [SerializeField]
    private GameObject[] drops;
    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    private List<GameObject> shooterEnemies = new List<GameObject>();

    [SerializeField]
    private float maxHP;
    private float currentHP;
    [SerializeField]
    private float bulletsLeft;

    private int enemiesInScene;
    private int currentWeaponID;

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    /* Text Popups */
    [SerializeField]
    private Text pickupPop;
    [SerializeField]
    private Text reloadPop;
    [SerializeField]
    private Text losePop;
    [SerializeField]
    private Text winPop;

    public void PickupPop(bool isPicking)
    {
        if (isPicking)
        {
            pickupPop.gameObject.SetActive(true);
        }
        else
        {
            pickupPop.gameObject.SetActive(false);
        }
    }

    private void ReloadPop()
    {
        if(bulletsLeft <= 0)
        {
            reloadPop.gameObject.SetActive(true);
        }
        else
        {
            reloadPop.gameObject.SetActive(false);
        }
    }

    private void LostPop()
    {
        if (myPlayer.IsDead())
        {
            losePop.gameObject.SetActive(true);
        }
        else
        {
            losePop.gameObject.SetActive(false);
        }
    }

    private void WonPop()
    {
        if(enemiesInScene <= 0)
        {
            winPop.gameObject.SetActive(true);
        }
        else
        {
            winPop.gameObject.SetActive(false);
        }
    }

    private void AddEnemies()
    {
        foreach (GameObject foundEnemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(foundEnemy);
        }
        Debug.Log("There is " + enemies.Count + " normal enemies in this level.");
    }

    private void AddShooterEnemies()
    {
        foreach (GameObject foundEnemy in GameObject.FindGameObjectsWithTag("EnemyShooter"))
        {
            shooterEnemies.Add(foundEnemy);
        }
        Debug.Log("There is " + shooterEnemies.Count + " enemies with weapons in this level.");
    }

    private void CheckEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            Enemy current = enemies[i].GetComponent<Enemy>();
            if (current.GetHP() <= 0)
            {
                enemies.Remove(enemies[i]);
                //Debug.Log("An enemy has died, removing from list...");
            }
        }
    }

    private void CheckShooterEnemies()
    {
        for (int i = 0; i < shooterEnemies.Count; i++)
        {
            ShooterEnemy current = shooterEnemies[i].GetComponent<ShooterEnemy>();
            if (current.GetHP() <= 0)
            {
                shooterEnemies.Remove(shooterEnemies[i]);
                //Debug.Log("An enemy has died, removing from list...");
            }
        }
    }

    public GameObject GetWeaponDrops(int gunID)
    {
        return drops[gunID];
    }

    public void RecieveDamage(float damage)
    {
        currentHP -= damage;
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void UpdateStats()
    {
        currentWeaponID = playerHand.GetCurrentWeaponID();
        bulletsLeft = playerHand.GetBulletsLeft();
        enemiesInScene = enemies.Count + shooterEnemies.Count;
        healthBar.value = currentHP;
        currentWeapon.sprite = GetWeaponDrops(currentWeaponID).GetComponent<SpriteRenderer>().sprite;
        string markerInfo = bulletsLeft + "|" + playerHand.GetCurrentWeaponMagSize();
        //Debug.Log(markerInfo);
        bulletsMarker.text = markerInfo;
        ReloadPop();
        LostPop();
        WonPop();
    }

    private void Awake()
    {
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        if (myPlayer == null)
        {
            myPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        playerHand = myPlayer.handReference;
        AddEnemies();
        AddShooterEnemies();
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    
    private void Update () {
        UpdateStats();
        if (currentHP <= 0)
        {
            myPlayer.Die();
        }
        CheckEnemies();
        CheckShooterEnemies();
        Debug.Log(enemiesInScene + " current enemies");
    }
}
