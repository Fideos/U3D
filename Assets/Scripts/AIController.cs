using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject target;
    public Transform[] path;

    private FSM fsm;

    public void SetTransition(TransitionID t)
    {
        fsm.PerformTransition(t);
    }
    
    public void Awake()
    {
        BuildFSM();
    }
    public void FixedUpdate()
    {
        fsm.CurrentState.Reason(target, gameObject);
        fsm.CurrentState.Behaviour(target, gameObject);
    }

    private void BuildFSM()
    {
        IdleState idle = new IdleState();
        idle.AddTransition(TransitionID.StartRoaming, StateID.Roaming);
        idle.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        idle.AddTransition(TransitionID.StartHurt, StateID.Hurt);

        RoamState roam = new RoamState();
        roam.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);
        roam.AddTransition(TransitionID.StopRoaming, StateID.Idle);
        roam.AddTransition(TransitionID.StartHurt, StateID.Hurt);

        ChaseTargetState chaseTarget = new ChaseTargetState();
        chaseTarget.AddTransition(TransitionID.LostTarget, StateID.Roaming);
        chaseTarget.AddTransition(TransitionID.StartHurt, StateID.Hurt);

        HurtState hurt = new HurtState();
        hurt.AddTransition(TransitionID.StopHurt, StateID.ChasingTarget);

        fsm = new FSM();
        fsm.AddState(roam);
        fsm.AddState(idle);
        fsm.AddState(chaseTarget);
        fsm.AddState(hurt);
    }
    
}

public class IdleState : FSMState
{
    float timer = 10;

    public IdleState()
    {
        myID = StateID.Idle;
    }

    private bool Sight(GameObject thisGameObject)
    {
        bool sighted = Physics.Raycast(thisGameObject.transform.position, Vector3.right, 15f);
        return sighted;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartRoaming);
            Debug.Log("Started Roaming...");
        }

        if (Sight(thisGameObject))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
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

    public RoamState()
    {
        myID = StateID.Roaming;
    }

    private bool Sight(GameObject thisGameObject)
    {
        bool sighted = Physics.Raycast(thisGameObject.transform.position, Vector3.right, 15f);
        return sighted;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            timer = 20;
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StopRoaming);
            Debug.Log("Started Idleing...");
        }

        if (Sight(thisGameObject))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        timer -= Time.deltaTime;
        Debug.Log("Roaming...");
    }
}

public class ChaseTargetState : FSMState
{
    public ChaseTargetState()
    {
        myID = StateID.ChasingTarget;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (Vector3.Distance(thisGameObject.transform.position, target.transform.position) >= 15f)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.LostTarget);
            Debug.Log("?");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StartHurt);
            Debug.Log("Got Attacked!...");
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        Debug.Log("Chasing...");
    }
}

public class HurtState : FSMState
{
    float timer = 2;

    public HurtState()
    {
        myID = StateID.Hurt;
    }

    public override void Reason(GameObject target, GameObject thisGameObject)
    {
        if (timer <= 0)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.StopHurt);
            Debug.Log("!");
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        timer -= Time.deltaTime;
        Debug.Log("Hurting...");
    }
}
