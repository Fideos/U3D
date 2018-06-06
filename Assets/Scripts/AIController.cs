using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIController : MonoBehaviour
{
    [SerializeField]
    private Enemy self;
    [SerializeField]
    public float runSpeed;
    [SerializeField]
    public float walkSpeed;

    public GameObject target; //El objetivo. En este caso Player.
    public Transform[] path;
    [SerializeField]
    private float changePathTime;

    private FSM fsm;

    //
    private SphereCollider enemyCollider;
    [SerializeField]
    private float hearingRadio;
    [SerializeField]
    private float fieldOfView;
    [SerializeField]
    private float fieldOfViewRadio;
    private bool hearing;
    private bool inSight;
    private bool inAttackRange;
    public Animator anim;

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
                        if(Physics.Raycast(this.transform.position, direction, out meleeAtackHit, 2))
                        {
                            if(meleeAtackHit.collider.tag == "Player")
                            {
                                Debug.Log("ReadyToAttack");
                            }
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
                        if (Physics.Raycast(this.transform.position, direction, out meleeAtackHit, 2))
                        {
                            if (meleeAtackHit.collider.tag == "Player")
                            {
                                Debug.Log("ReadyToAttack");
                            }
                        }
                    }
                }
            }
        }
    }
    //

    public void SetTransition(TransitionID t) //Le envia transiciones a PerformTransition.
    {
        fsm.PerformTransition(t); //Asigna el estado al que lleva la trancision a current.
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

    private void BuildFSM() //Crea los estados, y las trancisiones que llevan a otros estados.
    {
        IdleState idle = new IdleState(this);
        idle.AddTransition(TransitionID.StartRoaming, StateID.Roaming);
        idle.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        idle.AddTransition(TransitionID.StartHurt, StateID.Hurt);
        idle.AddTransition(TransitionID.Dying, StateID.Dead);

        RoamState roam = new RoamState(this);
        roam.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        roam.AddTransition(TransitionID.StopRoaming, StateID.Idle);
        roam.AddTransition(TransitionID.StartHurt, StateID.Hurt);
        roam.AddTransition(TransitionID.Dying, StateID.Dead);

        ChaseTargetState chaseTarget = new ChaseTargetState(this);
        chaseTarget.AddTransition(TransitionID.LostTarget, StateID.Roaming);
        chaseTarget.AddTransition(TransitionID.StartHurt, StateID.Hurt);
        chaseTarget.AddTransition(TransitionID.Dying, StateID.Dead);

        HurtState hurt = new HurtState(this);
        hurt.AddTransition(TransitionID.StopHurt, StateID.ChasingTarget);
        hurt.AddTransition(TransitionID.Dying, StateID.Dead);

        DeadState dead = new DeadState(this);
        dead.AddTransition(TransitionID.Healed, StateID.ChasingTarget);

        fsm = new FSM();
        fsm.AddState(roam);
        fsm.AddState(idle);
        fsm.AddState(chaseTarget);
        fsm.AddState(hurt);
        fsm.AddState(dead);
    }
    
}

public class IdleState : FSMState
{
    float timer = 10;

    public IdleState(AIController _aiController)
    {
        aiController = _aiController;
        myID = StateID.Idle;
    }


    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartRoaming);
            Debug.Log("Started Roaming...");
        }

        if (aiController.HeardSomething())
        {
            aiController.Rotate();
            Debug.Log("Hearing Things...");
        }

        if (aiController.SeenTarget())
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (aiController.isHit())
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }

        if (aiController.GetStatus() <= 0)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.Dying);
        }

    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        timer -= Time.deltaTime;
        Debug.Log("Idleing...");
    }

}

public class RoamState : FSMState
{

    float timer = 25;

    public RoamState(AIController _aiController)
    {
        aiController = _aiController;
        myID = StateID.Roaming;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            timer = 20; //Reseteo timer (Por que se rompía.).
            aiController.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StopRoaming);
            Debug.Log("Started Idleing...");
        }

        if (aiController.HeardSomething())
        {
            Debug.Log("Hearing Things...");
        }

        if (aiController.SeenTarget())
        {
            aiController.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (aiController.isHit())
        {
            aiController.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }

        if (aiController.GetStatus() <= 0)
        {
            aiController.anim.SetBool("isRoaming", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiController.anim.SetBool("isRoaming", true);
        aiController.Roam();
        timer -= Time.deltaTime;
        Debug.Log("Roaming...");
    }
}

public class ChaseTargetState : FSMState
{
    public ChaseTargetState(AIController _aiController)
    {
        aiController = _aiController;
        myID = StateID.ChasingTarget;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (Vector3.Distance(thisGameObject.transform.position, target.transform.position) >= 15f)
        {
            aiController.anim.SetBool("isRunning", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.LostTarget);
            Debug.Log("?");
        }

        if (aiController.isHit())
        {
            aiController.anim.SetBool("isRunning", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }

        if (aiController.GetStatus() <= 0)
        {
            aiController.anim.SetBool("isRunning", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiController.anim.SetBool("isRunning", true);
        aiController.ChaseTarget();
        Debug.Log("Chasing...");
    }
}

public class HurtState : FSMState
{
    float timer = 0.6f;

    public HurtState(AIController _aiController)
    {
        aiController = _aiController;
        myID = StateID.Hurt;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            aiController.anim.SetBool("gotHit", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StopHurt);
            Debug.Log("!");
        }

        if (aiController.GetStatus() <= 0)
        {
            aiController.anim.SetBool("gotHit", false);
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.Dying);
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        aiController.anim.SetBool("gotHit", true);
        timer -= Time.deltaTime;
        Debug.Log("Hurting...");
    }
}

public class DeadState : FSMState
{

    public DeadState(AIController _aiController)
    {
        aiController = _aiController;
        myID = StateID.Dead;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if(aiController.GetStatus() > 0)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.Healed);
        }
        else
        {
            aiController.Die();
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        Debug.Log("Dead");
    }
}
