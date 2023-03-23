using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [HideInInspector]
    private Rigidbody2D _rigidBody;
    private List<Entity> entities;

    private Vector3 lastPosition;
    private Vector3 _localVelocity;

    [SerializeField]
    private Vector3 initialImpulse;
    [SerializeField]
    private bool debug = false;

    public Rigidbody2D RigidBody { get => _rigidBody; protected set => _rigidBody = value; }
    public Vector3 LocalVelocity { get => _localVelocity; set => _localVelocity = value; }

    void Awake()
    {
        lastPosition = transform.position;

        entities = new List<Entity>();

        if(!TryGetComponent<Rigidbody2D>(out _rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
            RigidBody.gravityScale = 0;
        }

        RigidBody.AddForce(initialImpulse);
        LocalVelocity = transform.position - lastPosition;
    }

    // Update is called once per frame
    void Update()
    {
        LocalVelocity = transform.position - lastPosition;
        foreach(Entity entity in entities) {
            entity.transform.Translate(LocalVelocity, transform);
            //entity.transform.position += LocalVelocity;
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Entity entity;
        if(other.gameObject.TryGetComponent<Entity>(out entity)){
            entities.Add(entity);
            // Set velocity relative to station
            entity.RigidBody.velocity = entity.RigidBody.velocity - RigidBody.velocity;
        }

        if(debug) Debug.Log("Entered influence of " + gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Entity entity;
        if(other.gameObject.TryGetComponent<Entity>(out entity)){
            entities.Remove(entity);
            entity.RigidBody.velocity = entity.RigidBody.velocity + RigidBody.velocity;
        }

        if(debug) Debug.Log("Left influence of " + gameObject.name);
    }
}
