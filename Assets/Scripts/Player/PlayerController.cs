using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : Controllable
{
    private Rigidbody2D rb;
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

        if(!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // TODO Jank, need to ensure reference to sprite
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        // TODO Move to input controller
        if(Input.GetKeyDown(KeyCode.E) && currentVehicle != null) {
            ExitVehicle();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Debug.Log("Movement: " + Input.GetAxisRaw("Horizontal") + " | " + Input.GetAxisRaw("Vertical"));
        coordinates.text = transform.position.x.ToString("#.##") + " | " + transform.position.y.ToString("#.##");
        if(currentVehicle) {
            if(currentVehicle.rb.velocity.magnitude > 0) {
                speed.text = currentVehicle.rb.velocity.magnitude.ToString("#.##");
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
            rb.AddForce(direction * moveSpeed);
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
            transform.position = vehicle.transform.position;
            transform.parent = vehicle.transform;
            rb.simulated = false;
            playerSprite.enabled = false;
            vehicle.Posess();
            posessed = false;
        }
    }

    public void ExitVehicle()
    {
        if(currentVehicle != null) {
            transform.position = currentVehicle.dismountAnchor.position;
            transform.parent = null;
            rb.simulated = true;
            if(currentStation) {
                rb.velocity = currentVehicle.rb.velocity + currentStation.rb.velocity;
            } else {
                rb.velocity = currentVehicle.rb.velocity;
            }
            playerSprite.enabled = true;
            currentVehicle.UnPosess();
            currentVehicle = null;
            posessed = true;

            cameraController.TargetSize = 10;
            cameraController.ZoomSpeed = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Gravity") {
            grounded = true;
            currentStation = other.gameObject.GetComponent<Station>();

            // if(other.gameObject.TryGetComponent<Station>(out currentStation)) {

            // }
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

            if(currentStation != null && currentStation.gameObject == other.gameObject) {
                currentStation = null;
            }
        }
    }
}