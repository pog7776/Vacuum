using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : Controllable
{
    private SpriteRenderer playerSprite;
    private Vehicle currentVehicle = null;
    private bool grounded = true;
    [HideInInspector]
    public Station currentStation;
    public float walkSpeedMod = 10f;

    public TMP_Text coordinates;
    public TMP_Text speed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Posess();

        if(!TryGetComponent<Rigidbody2D>(out _rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
        }

        // TODO Jank, need to ensure reference to sprite
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Debug.Log("Movement: " + Input.GetAxisRaw("Horizontal") + " | " + Input.GetAxisRaw("Vertical"));
        coordinates.text = transform.position.x.ToString("#.##") + " | " + transform.position.y.ToString("#.##");
        if(currentVehicle) {
            if(currentVehicle.RigidBody.velocity.magnitude > 0) {
                speed.text = currentVehicle.RigidBody.velocity.magnitude.ToString("#.##");
            } else {
                speed.text = "0.00";
            }
        }
    }

    protected override void Move(Vector3 direction)
    {
        if(grounded) {
            transform.localPosition = transform.localPosition + (direction * (moveSpeed / walkSpeedMod));
        }
        else {
            RigidBody.AddForce(direction * moveSpeed);
        }
    }

    // private void MoveSpace(Vector3 direction)
    // {
    //     rb.AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * (moveSpeed * direction.y));
    //     rb.AddRelativeForce(new Vector3(0, -direction.x, 0) * moveSpeed);
    // }

    public void EnterVehicle(Vehicle vehicle)
    {
        if(currentVehicle == null) {
            currentVehicle = vehicle;
            // TODO seat anchor position on the vehicle
            transform.position = vehicle.transform.position;
            transform.parent = vehicle.transform;
            RigidBody.simulated = false;
            playerSprite.enabled = false;
            vehicle.Posess();
            cameraController.FollowTarget = currentVehicle.gameObject;
            UnPosess();
        }
    }

    public void OnExitVehicle(InputValue value) => ExitVehicle();

    public void ExitVehicle()
    {
        if(currentVehicle != null) {
            transform.position = currentVehicle.dismountAnchor.position;
            transform.parent = null;
            RigidBody.simulated = true;
            if(currentVehicle.currentStation) {
                RigidBody.velocity = currentVehicle.currentStation.RigidBody.velocity;
            } else {
                RigidBody.velocity = currentVehicle.RigidBody.velocity;
            }
            playerSprite.enabled = true;
            currentVehicle.UnPosess();
            currentVehicle = null;
            Posess();

            cameraController.TargetSize = 10;
            cameraController.ZoomSpeed = 2;
            cameraController.FollowTarget = gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Gravity") {
            grounded = true;

            if(!other.gameObject.TryGetComponent<Station>(out currentStation)) {
                currentStation = other.gameObject.GetComponentInParent<Station>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            grounded = false;
            // rb.drag = 0;
            // if(rb.velocity.magnitude == 0) {
            //     rb.AddForce(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * (moveSpeed * (walkSpeedMod * 10f)));
            // } else {
            //     rb.AddForce(rb.velocity);
            // }

            // TODO Really need to figure this out
            // If a station is within another station
            // There was another issue i saw but i cant think of it right now
            if(currentStation != null) { //&& currentStation.gameObject == other.gameObject) {
                currentStation = null;
            }
        }
    }

    public void OnCameraFocus(InputValue value) {
        if(value.Get<float>() != 0) {
            cameraController.offsetCamera = true;

            cameraController.FixCamType = FixCamMode.FixedUpdate;
            
            // This is a mess ♥
            // Turns out i wasnt setting the ship to the follow target
            // So don't need the NormalUpdate
            // if(grounded || currentVehicle.currentStation) {
            //     cameraController.FixCamType = FixCamMode.FixedUpdate;    
            // } else {
            //     cameraController.FixCamType = FixCamMode.NormalUpdate;
            // }

        } else {
            cameraController.offsetCamera = false;
            cameraController.FixCamType = FixCamMode.LateUpdate;
        }
    }
}