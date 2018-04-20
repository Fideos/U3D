using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionID
{
    NullTransition = 0,
    StartRoaming,
    StopRoaming,
    SawTarget,
    LostTarget,
    StartHurt,
    StopHurt,


}

public enum StateID
{
    NullState = 0,
    Idle,
    Roaming,
    ChasingTarget,
    Hurt,
}

public abstract class FSMState
{
    protected Dictionary<TransitionID, StateID> stateDictionary = new Dictionary<TransitionID, StateID>();
    protected StateID myID;
    public StateID getID
    {
        get
        {
            return myID;
        }
    }

    public void AddTransition(TransitionID transition, StateID state)
    {
        if (transition == TransitionID.NullTransition)
        {
            Debug.LogError("!");
            return;
        }

        if (state == StateID.NullState)
        {
            Debug.LogError("!");
            return;
        }
        if (stateDictionary.ContainsKey(transition))
        {
            Debug.LogError("El estado ya tiene esta trancisión asignada.");
        }
        else
        {
            stateDictionary.Add(transition, state);
        }
    }


    public void DeleteTransition(TransitionID transition)
    {
        if (transition == TransitionID.NullTransition)
        {
            Debug.LogError("!");
            return;
        }
        if (stateDictionary.ContainsKey(transition))
        {
            stateDictionary.Remove(transition);
        }
        else
        {
            Debug.LogError("La transicion no se encuentra en la lista.");
        }
    }

    public StateID GetOutputState(TransitionID transition)
    {
        if (stateDictionary.ContainsKey(transition))
        {
            return stateDictionary[transition];
        }
        else
        {
            return StateID.NullState;
        }
    }

    public abstract void Reason(GameObject target, GameObject thisGameObject);

    public abstract void Behaviour(GameObject target, GameObject thisGameObject);
}

public class FSM
{
    private List<FSMState> stateList;
    private FSMState currentState;
    public FSMState CurrentState
    {
        get
        {
            return currentState;
        }
    }
    private StateID currentID;
    public StateID CurrentID
    {
        get
        {
            return currentID;
        }
    }

    public FSM() //Inicializa la lista de estados
    {
        stateList = new List<FSMState>();
    }

    public void AddState(FSMState state) //Ejemplo de la wiki
    {

        if (state == null)
        {
            Debug.LogError(" ! Estado nulo.");
        }
        
        if (stateList.Count == 0)
        {
            stateList.Add(state);
            currentState = state;
            currentID = state.getID;
            return;
        }
        
        foreach (FSMState s in stateList)
        {
            if (s.getID == state.getID)
            {
                Debug.LogError("El estado ya existe.");
                return;
            }
        }
        stateList.Add(state);
    }

    public void DeleteState(StateID state)
    {
        if (state == StateID.NullState)
        {
            Debug.LogError(" ! Estado nulo.");
            return;
        }

        foreach (FSMState s in stateList)
        {
            if (s.getID == state)
            {
                stateList.Remove(s);
                return;
            }
        }
        Debug.LogError("El estado no existe.");
    }

    public void PerformTransition(TransitionID transition)
    {
        
        if (transition == TransitionID.NullTransition)
        {
            Debug.LogError("Transición nula.");
            return;
        }
        
        StateID state = currentState.GetOutputState(transition);
        
        if (state == StateID.NullState)
        {
            Debug.LogError("La trancición no lleva a ningun estado.");
            return;
        }

        currentID = state;
        foreach (FSMState s in stateList)
        {
            if (s.getID == currentID)
            {
                currentState = s;

                break;
            }
        }

    }
}
