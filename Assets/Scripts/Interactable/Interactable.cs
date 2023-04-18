using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// TODO Make an IInteractable so I can extend it for custom interactables
// Or should I just inherit from it?
// Or both?
public class Interactable : MonoBehaviour {
    [SerializeField]
    private bool debug = false;

    [SerializeField]
    private UnityEvent _onInteract;
    public UnityEvent OnInteract
    {
        get { return _onInteract; }
    }
    
    [SerializeField]
    private UnityEvent _onHoverEnter;
    public UnityEvent OnHoverEnter
    {
        get { return _onHoverEnter; }
    }

    [SerializeField]
    private UnityEvent _onHover;
    public UnityEvent OnHover
    {
        get { return _onHover; }
    }

    [SerializeField]
    private UnityEvent _onHoverExit;
    public UnityEvent OnHoverExit
    {
        get { return _onHoverExit; }
    }

    public void Interact() {
        if(debug) Debug.Log(gameObject.name + " Interact");
        OnInteract?.Invoke();
    }

    public void InvokeOnEnter() {
        if(debug) Debug.Log(gameObject.name + " OnEnter");
        OnHoverEnter?.Invoke();
    }

    public void InvokeOnExit() {
        if(debug) Debug.Log(gameObject.name + " OnExit");
        OnHoverExit?.Invoke();
    }

    public void InvokeOnHover() {
        //if(debug) Debug.Log(gameObject.name + " OnHover");
        OnHover?.Invoke();
    }
}
