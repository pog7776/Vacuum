using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controllable : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 1.0f;

    protected bool posessed = false;

    [SerializeField]
    protected Vector3 lookatOffset = Vector3.zero;

    protected CameraController cameraController;

    protected Rigidbody2D _rigidBody;

    private Vector2 moveDirection = Vector2.zero;

    public Rigidbody2D RigidBody { get => _rigidBody; protected set => _rigidBody = value; }
    public Vector2 MoveDirection { get => moveDirection; protected set => moveDirection = value; }

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
            LookAt(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            //LockCamera(Camera.main);
        }
    }

    protected virtual void FixedUpdate() {
        if(posessed) {
            Move(MoveDirection);

            // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // Vector3 dir = (transform.position - mousePos).normalized;
            // Debug.DrawLine(transform.position, transform.position - (dir * 2), Color.magenta);
        }
    }

    public void OnMove(InputValue value) => MoveDirection = value.Get<Vector2>();

    protected virtual void Move(Vector3 direction)
    {
        transform.localPosition = transform.localPosition + (direction * (moveSpeed / 10f));
    }

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
