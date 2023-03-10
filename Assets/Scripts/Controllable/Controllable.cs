using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 1.0f;

    protected bool posessed = false;

    [SerializeField]
    protected Vector3 lookatOffset = Vector3.zero;

    protected CameraController cameraController;

    protected Rigidbody2D _rigidBody;

    public Rigidbody2D RigidBody { get => _rigidBody; protected set => _rigidBody = value; }

    private void Awake() {
        if(!TryGetComponent<Rigidbody2D>(out _rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    protected virtual void Update() {
        if(posessed) {
            LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //LockCamera(Camera.main);
        }
    }

    protected virtual void FixedUpdate() {
        if(posessed && IsMoveInput())
        {
            Move(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),0));
        }
    }

    protected virtual void Move(Vector3 direction)
    {
        transform.localPosition = transform.localPosition + (direction * (moveSpeed / 10f));
    }

    // Needs to be handeled by the camera so
    // multiple objects can't be the target
    // protected virtual void LockCamera(Camera camera)
    // {
    //     Vector3 newPos = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
    //     camera.transform.position = newPos;
    // }

    // TODO Need to move to new input system and not do this every frame crap
    protected virtual bool IsMoveInput() => (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

    protected virtual void LookAt(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, Controllable.AngleBetween(transform.position, position));
        rotation.eulerAngles += lookatOffset;
        transform.rotation = rotation;
    }

    // TODO Move to static tool class
    public static float AngleBetween(Vector3 entity, Vector3 target)
    {
        float AngleRad = Mathf.Atan2(target.y - entity.y, target.x - entity.x);
        // Get Angle in Degrees
        return (180 / Mathf.PI) * AngleRad;
    }

    public virtual void Posess() {
        posessed = true;
        // TODO make a follow target transform for a controllable
        cameraController.FollowTarget = gameObject;
    }

    public virtual void UnPosess() {
        posessed = false;
    }
}
