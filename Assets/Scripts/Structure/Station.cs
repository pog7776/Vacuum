using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [HideInInspector]
    private Rigidbody2D _rigidBody;
    private List<Entity> entities;

    private Vector3 lastPosition;

    [SerializeField]
    private Vector3 initialImpulse;

    public Rigidbody2D RigidBody { get => _rigidBody; protected set => _rigidBody = value; }

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        entities = new List<Entity>();

        if(!TryGetComponent<Rigidbody2D>(out _rigidBody))
        {
            RigidBody = gameObject.AddComponent<Rigidbody2D>();
            RigidBody.gravityScale = 0;
        }

        RigidBody.AddForce(initialImpulse);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = transform.position - lastPosition;
        foreach(Entity entity in entities) {
            entity.transform.Translate(velocity, transform);
            //entity.transform.position += velocity;
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

        Debug.Log("Entered influence of " + gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Entity entity;
        if(other.gameObject.TryGetComponent<Entity>(out entity)){
            entities.Remove(entity);
            entity.RigidBody.velocity = entity.RigidBody.velocity + RigidBody.velocity;
        }

        Debug.Log("Left influence of " + gameObject.name);
    }
}
