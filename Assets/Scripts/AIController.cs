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
        RoamState roam = new RoamState();
        roam.AddTransition(TransitionID.SawTarget, StateID.ChasingTarget);

        ChaseTargetState chaseTarget = new ChaseTargetState();
        chaseTarget.AddTransition(TransitionID.LostTarget, StateID.Roaming);

        fsm = new FSM();
        fsm.AddState(roam);
        fsm.AddState(chaseTarget);
    }
    
}

public class RoamState : FSMState
{

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
        if (Sight(thisGameObject))
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.SawTarget);
            Debug.Log("!");
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
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
        if(Vector3.Distance(thisGameObject.transform.position, target.transform.position) >= 15f)
        {
            thisGameObject.GetComponent<AIController>().SetTransition(TransitionID.LostTarget);
            Debug.Log("?");
        }
    }

    public override void Behaviour(GameObject target, GameObject thisGameObject)
    {
        Debug.Log("Chasing...");
    }
}
