using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {
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
        Debug.Log(gameObject.name + " Interact");
        OnInteract?.Invoke();
    }

    public void InvokeOnEnter() {
        Debug.Log(gameObject.name + " OnEnter");
        OnHoverEnter?.Invoke();
    }

    public void InvokeOnExit() {
        Debug.Log(gameObject.name + " OnExit");
        OnHoverExit?.Invoke();
    }

    public void InvokeOnHover() {
        //Debug.Log(gameObject.name + " OnHover");
        OnHover?.Invoke();
    }
}
