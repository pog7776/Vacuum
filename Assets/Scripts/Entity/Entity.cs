using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool grounded = true;

    [SerializeField]
    private float groundedDrag = 1;
    
    //public bool playerControlled = false;
    private PlayerController controller;

    //[SerializeField]
    //private bool physicsObject = true;

    // Start is called before the first frame update
    void Start() {
        if(!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        controller = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Gravity") {
            grounded = true;
            //rb.velocity = Vector3.zero;
            rb.drag = groundedDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            grounded = false;
            //rb.AddForce();
            rb.drag = 0;

            if(controller != null && rb.velocity.magnitude <= 5) {
                Vector3 force = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (controller.moveSpeed * (controller.walkSpeedMod * 10f));
                //Debug.Log("Force: " + force.x + " | " + force.y + " | " + force.z);
                rb.AddForce(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (controller.moveSpeed * (controller.walkSpeedMod * 10f)));
            } else {
                rb.AddForce(rb.velocity);
            }
        }
    }

    // Add gravity well to litst on entity so camera can get player and gravity bodies affecting it in frame
}
