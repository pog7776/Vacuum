using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRay : MonoBehaviour {

    #region Public Variables
    public Transform raycastOrigin;
    public float rayDistance = 20f;

    public KeyCode interactKey = KeyCode.E;

    public ContactFilter2D contactFilter;

    #endregion

    #region Private Variables
    private Ray ray;
    private List<RaycastHit2D> hit;
    private Interactable interactable;

    #endregion

    // Start is called before the first frame update
    void Start() {
        hit = new List<RaycastHit2D>();
        //ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        ray = new Ray(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,0,1));

        // TODO do i want to do it this way or do it though unity input?
        //InputMap.Instance.AddDelegate(interactKey, delegate { interactable?.Interact(); });
    }

    // Update is called once per frame
    void Update() {
        // Update Raycast
        //ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        ray = new Ray(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,0,rayDistance));
        //Debug.Log("Mouse Pos: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // Check if Raycast hits an Interactable
        HandleInteractable();
    }

    void FixedUpdate() {
        
    }

    private void HandleInteractable() {
        Interactable previousInteractable = interactable;

        // Cast Ray
        //if(Physics.Raycast(ray, out hit, rayDistance)) {
        if(Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,0,1), contactFilter, hit, rayDistance) > 0) {

            // If new Object
            if(previousInteractable == null || hit[0].collider.gameObject != previousInteractable.gameObject) {
                if(hit[0].collider.gameObject.TryGetComponent<Interactable>(out interactable)) {
                    // Invoke OnEnter Events
                    interactable?.InvokeOnEnter();
                    // Invoke OnExit Events
                    previousInteractable?.InvokeOnExit();
                }
                else {
                    // Set null if no interactable found
                    interactable = null;
                    previousInteractable?.InvokeOnExit();
                }
            }
        }
        else {
            // Set to null if no object found
            interactable = null;
            previousInteractable?.InvokeOnExit();
        }

        // if(Input.GetButtonDown("Interact")) {
        //     interactable?.Interact();
        // }

        if(Input.GetMouseButtonDown(0)) {
            interactable?.Interact();
        }

        // Invoke OnHover Events
        interactable?.InvokeOnHover();
    }

    private void OnDrawGizmos() {
        Gizmos.color = interactable ? Color.green : Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);
    }

}
