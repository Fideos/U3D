using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooter : MonoBehaviour
{

    [SerializeField]
    private ShooterEnemy self;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Transform[] path;
    [SerializeField]
    private float changePathTime;

    private SphereCollider enemyCollider;
    [SerializeField]
    private float hearingRadio;
    [SerializeField]
    private float fieldOfView;
    [SerializeField]
    private float fieldOfViewRadio;
    [SerializeField]
    private float attackRange;
    private bool hearing;
    private bool inSight;
    private bool inAttackRange;
    public Animator anim;

    public void Reload()
    {
        self.Reload();
    }

    public bool OutOfBullets()
    {
        return self.GunLoaded();
    }

    public void Attack()
    {
        self.Attack(target.transform);
    }

    public void Rotate()
    {
        self.Rotate(target.transform);
    }

    public bool isHit() //Hurt
    {
        if (self.getHit())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChaseTarget() //Chasing
    {
        self.Movement(target.transform, runSpeed);
    }

    /* Variables de Roaming */
    private int point = 0;

    private float roamTimer;

    public void Roam() //Roaming
    {
        roamTimer -= Time.deltaTime;
        if (point < path.Length)
        {
            self.Movement(path[point], walkSpeed);
            if (roamTimer <= 0)
            {
                point++;
                roamTimer = changePathTime;
            }
        }
        else
        {
            point = 0;
        }
    }

    public void Die()
    {
        anim.SetBool("isDead", true);
        self.Die();
    }

    public float GetStatus()
    {
        return self.GetHP();
    }

    public bool HeardSomething()
    {
        return hearing;
    }

    public bool inRange()
    {
        return inAttackRange;
    }

    public bool SeenTarget()
    {
        return inSight;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            hearing = true;
            target = other.gameObject;
            Vector3 direction = target.transform.position - transform.position;
            if (fieldOfView / 2 >= Vector3.Angle(this.transform.forward, direction))
            {
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position, direction, out hit, fieldOfViewRadio))
                {
                    if (hit.collider.tag == "Player")
                    {
                        inSight = true;
                        RaycastHit meleeAtackHit;
                        if (Physics.Raycast(this.transform.position, direction, out meleeAtackHit, attackRange))
                        {
                            if (meleeAtackHit.collider.tag == "Player")
                            {
                                inAttackRange = true;
                                Debug.Log("InRange");
                            }
                        }
                        else
                        {
                            inAttackRange = false;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hearing = false;
            inSight = false;
            inAttackRange = false;
            //target = null;
            Vector3 direction = target.transform.position - transform.position;
            if (fieldOfView / 2 >= Vector3.Angle(Vector3.forward, direction))
            {
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position, direction, out hit, fieldOfViewRadio))
                {
                    if (hit.collider.tag == "Player")
                    {
                        inSight = true;
                        RaycastHit meleeAtackHit;
                        if (Physics.Raycast(this.transform.position, direction, out meleeAtackHit, attackRange))
                        {
                            if (meleeAtackHit.collider.tag == "Player")
                            {
                                Debug.Log("ReadyToAttack");
                            }
                        }
                        else
                        {
                            inAttackRange = false;
                        }
                    }
                }
            }
        }
    }

    private FSM fsm;


    public void SetTransition(TransitionID t)
    {
        fsm.PerformTransition(t);
    }

    public void Awake()
    {
        enemyCollider = this.GetComponent<SphereCollider>();
        anim = this.GetComponent<Animator>();
        enemyCollider.radius = hearingRadio;
        target = GameObject.FindGameObjectWithTag("Player");
        roamTimer = changePathTime;
        BuildFSM();
    }

    public void FixedUpdate()
    {
        fsm.CurrentState.Reason(target, gameObject);
        fsm.CurrentState.Behaviour(target, gameObject);
    }

    private void BuildFSM()
    {
        ShooterIdleState idle = new ShooterIdleState(this);
        idle.AddTransition(TransitionID.StartRoaming, StateID.Roaming);
        idle.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        idle.AddTransition(TransitionID.StartHurt, StateID.Hurt);
        idle.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterRoamState roam = new ShooterRoamState(this);
        roam.AddTransition(TransitionID.StopRoaming, StateID.Idle);
        roam.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        roam.AddTransition(TransitionID.StartHurt, StateID.Hurt);
        roam.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterChaseState chaseTarget = new ShooterChaseState(this);
        chaseTarget.AddTransition(TransitionID.LostTarget, StateID.Roaming);
        chaseTarget.AddTransition(TransitionID.InRange, StateID.Attack);
        chaseTarget.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterAttackState attack = new ShooterAttackState(this);
        attack.AddTransition(TransitionID.OutRange, StateID.ChasingTarget);
        attack.AddTransition(TransitionID.Unloaded, StateID.Reload);
        attack.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterReloadState reload = new ShooterReloadState(this);
        reload.AddTransition(TransitionID.Loaded, StateID.ChasingTarget);
        reload.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterHurtState hurt = new ShooterHurtState(this);
        hurt.AddTransition(TransitionID.StopHurt, StateID.ChasingTarget);
        hurt.AddTransition(TransitionID.Dying, StateID.Dead);

        ShooterDeadState dead = new ShooterDeadState(this);
        dead.AddTransition(TransitionID.Healed, StateID.ChasingTarget);

        fsm = new FSM();
        fsm.AddState(idle);
        fsm.AddState(roam);
        fsm.AddState(chaseTarget);
        fsm.AddState(attack);
        fsm.AddState(reload);
        fsm.AddState(hurt);
        fsm.AddState(dead);
    }

}

public class ShooterIdleState : FSMState
{

    float timer = 20;

    public ShooterIdleState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Idle;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            timer = 20;
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.StartRoaming);
            Debug.Log("Shooter started Roaming...");
        }

        if (aiShooter.SeenTarget())
        {
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (aiShooter.isHit())
        {
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        timer -= Time.deltaTime;
        Debug.Log("Shooter Idleing...");
    }
}

public class ShooterRoamState : FSMState
{

    float timer = 10;

    public ShooterRoamState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Roaming;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            aiShooter.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.StopRoaming);
            Debug.Log("Shooter started Idleing...");
        }

        if (aiShooter.SeenTarget())
        {
            aiShooter.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (aiShooter.isHit())
        {
            aiShooter.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.anim.SetBool("isRoaming", false);
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiShooter.anim.SetBool("isRoaming", true);
        timer -= Time.deltaTime;
        Debug.Log("Shooter Roaming...");
    }
}

public class ShooterChaseState : FSMState
{

    public ShooterChaseState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.ChasingTarget;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (Vector3.Distance(thisGameObject.transform.position, target.transform.position) >= 15f)
        {
            aiShooter.anim.SetBool("isRunning", false);
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.LostTarget);
            Debug.Log("?");
        }

        if (aiShooter.inRange())
        {
            aiShooter.anim.SetBool("isRunning", false);
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.InRange);
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.anim.SetBool("isRunning", false);
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }

    }


    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiShooter.anim.SetBool("isRunning", true);
        aiShooter.ChaseTarget();
        Debug.Log("Chasing...");
    }
}

public class ShooterAttackState : FSMState
{

    public ShooterAttackState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Attack;
    }


    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (!aiShooter.inRange())
        {
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.OutRange);
        }

        if (aiShooter.OutOfBullets())
        {
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.Unloaded);
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiShooter.Rotate();
        aiShooter.Attack();
        Debug.Log("Shooting...");
    }
}

public class ShooterReloadState : FSMState
{

    float timer = 1.7f;

    public ShooterReloadState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Reload;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        
        if(timer <= 0)
        {
            aiShooter.anim.SetBool("isReloading", false);
            aiShooter.Reload();
            timer = 1.7f;
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.Loaded);
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.anim.SetBool("isReloading", false);
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiShooter.anim.SetBool("isReloading", true);
        timer -= Time.deltaTime;
        Debug.Log("Enemy reloading...");
    }
}


public class ShooterHurtState : FSMState
{

    float timer = 0.6f;

    public ShooterHurtState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Hurt;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if(timer <= 0)
        {
            aiShooter.anim.SetBool("gotHit", false);
            timer = 0.6f;
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.StopHurt);
        }

        if (aiShooter.GetStatus() <= 0)
        {
            aiShooter.anim.SetBool("gotHit", false);
            aiShooter.GetComponent<AIShooter>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiShooter.anim.SetBool("gotHit", true);
        timer -= Time.deltaTime;
        Debug.Log("Hurting...");
    }
}

public class ShooterDeadState : FSMState
{

    public ShooterDeadState(AIShooter _aiShooter)
    {
        aiShooter = _aiShooter;
        myID = StateID.Dead;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if(aiShooter.GetStatus() > 0)
        {
            thisGameObject.GetComponent<AIShooter>().SetTransition(TransitionID.Healed);
        }
        else
        {
            aiShooter.Die();
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        Debug.Log("Dead");
    }
}