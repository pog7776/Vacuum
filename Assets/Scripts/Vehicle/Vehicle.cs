using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : Controllable
{
    private float strafeMod = 0.5f;

    [SerializeField]
    private float turnSpeed = 1f;

    public Transform dismountAnchor;

    [HideInInspector]
    public Station currentStation;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        if(!TryGetComponent<Rigidbody2D>(out _rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
        }

        if(dismountAnchor == null) {
            dismountAnchor = new GameObject().transform;
            dismountAnchor.parent = transform;
            dismountAnchor.name = "GeneratedDismountAnchor";
            dismountAnchor.transform.localPosition = new Vector3(-1, 0, 0);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(posessed) {
            ZoomOnVelocity();
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        // if(posessed) {
        //     if(posessed && IsMoveInput())
        //     {
        //         Move(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),0));
        //     }

        //     LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // }

        //Debug.Log("Velocity: " + rb.velocity.magnitude);
    }

    protected override void Move(Vector3 direction) {
        //rb.AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * (moveSpeed * direction.y));
        RigidBody.AddForce(transform.up * (moveSpeed * direction.y));
        RigidBody.AddRelativeForce(new Vector3(direction.x, 0, 0) * (moveSpeed * strafeMod));
    }

    // public override void Posess() {
    //     posessed = true;
    // }

    // public override void UnPosess() {
    //     posessed = false;
    //     //rb.velocity = Vector3.zero;
    // }

    private void ZoomOnVelocity() {
        //Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 10 + (rb.velocity.magnitude / 2), Time.deltaTime);
        cameraController.TargetSize = 30 + (RigidBody.velocity.magnitude / 2);
        if(cameraController.ZoomSpeed != 1) {
            cameraController.ZoomSpeed = 1;
        }
    }

    protected override void LookAt(Vector3 position)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, Controllable.AngleBetween(transform.position, position));
        targetRotation.eulerAngles += lookatOffset;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        RigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed));
        //rb.MoveRotation(targetRotation);
        //rb.MoveRotation(Quaternion.Lerp(Quaternion.Euler(0,0,rb.rotation), targetRotation, Time.fixedDeltaTime * turnSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Gravity") {
            if(!other.gameObject.TryGetComponent<Station>(out currentStation)) {
                currentStation = other.gameObject.GetComponentInParent<Station>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            if(currentStation != null) {
                currentStation = null;
            }
        }
    }
}
