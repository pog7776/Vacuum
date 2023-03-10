using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;
    private List<Entity> entities;

    private Vector3 lastPosition;

    [SerializeField]
    private Vector3 initialImpulse;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        entities = new List<Entity>();

        if(!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        rb.AddForce(initialImpulse);
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
            entity.rb.velocity = entity.rb.velocity - rb.velocity;
        }

        Debug.Log("Entered influence of " + gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Entity entity;
        if(other.gameObject.TryGetComponent<Entity>(out entity)){
            entities.Remove(entity);
            entity.rb.velocity = entity.rb.velocity + rb.velocity;
        }

        Debug.Log("Left influence of " + gameObject.name);
    }
}
