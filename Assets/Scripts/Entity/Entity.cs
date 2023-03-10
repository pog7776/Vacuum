using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [HideInInspector]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private bool grounded = true;

    [SerializeField]
    private float groundedDrag = 1;

    private PlayerController playerController;

    public Rigidbody2D RigidBody { get => rigidBody; private set => rigidBody = value; }
    public bool Grounded { get => grounded; set => grounded = value; }
    public float GroundedDrag { get => groundedDrag; set => groundedDrag = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    //[SerializeField]
    //private bool physicsObject = true;

    // Start is called before the first frame update
    void Start() {
        if(!TryGetComponent<Rigidbody2D>(out rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
            RigidBody.gravityScale = 0;
        }

        PlayerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Gravity") {
            Grounded = true;
            //rb.velocity = Vector3.zero;
            RigidBody.drag = GroundedDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            Grounded = false;
            //rb.AddForce();
            RigidBody.drag = 0;

            if(PlayerController != null && RigidBody.velocity.magnitude <= 5) {
                //Vector3 force = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (PlayerController.moveSpeed * (PlayerController.walkSpeedMod * 10f));
                //Debug.Log("Force: " + force.x + " | " + force.y + " | " + force.z);
                RigidBody.AddForce(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (PlayerController.moveSpeed * (PlayerController.walkSpeedMod * 10f)));
            } else {
                RigidBody.AddForce(RigidBody.velocity);
            }
        }
    }

    // Add gravity well to litst on entity so camera can get player and gravity bodies affecting it in frame
}
