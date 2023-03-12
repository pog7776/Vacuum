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
    [SerializeField]
    private float flightDrag = 0;

    [SerializeField]
    private float groundedAngularDrag = 1f;
    [SerializeField]
    private float flightAngularDrag = 0.00f;

    private PlayerController playerController;

    public Rigidbody2D RigidBody { get => rigidBody; private set => rigidBody = value; }
    public bool Grounded { get => grounded; set => grounded = value; }
    public float GroundedDrag { get => groundedDrag; set => groundedDrag = value; }
    public float FlightDrag { get => flightDrag; set => flightDrag = value; }
    public float FlightAngularDrag { get => flightAngularDrag; set => flightAngularDrag = value; }
    public float GroundedAngularDrag { get => groundedAngularDrag; set => groundedAngularDrag = value; }
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
            RigidBody.angularDrag = GroundedAngularDrag;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            Grounded = false;
            //rb.AddForce();
            RigidBody.drag = FlightDrag;
            RigidBody.angularDrag = FlightAngularDrag;

            if(PlayerController != null && RigidBody.velocity.magnitude <= 5) {
                //Vector3 force = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (PlayerController.moveSpeed * (PlayerController.walkSpeedMod * 10f));
                //Debug.Log("Force: " + force.x + " | " + force.y + " | " + force.z);

                RigidBody.AddForce(playerController.MoveDirection * (PlayerController.moveSpeed * (PlayerController.walkSpeedMod * 10f)));
            } else {
                RigidBody.AddForce(RigidBody.velocity);
            }
        }
    }

    // Add gravity well to litst on entity so camera can get player and gravity bodies affecting it in frame
}
