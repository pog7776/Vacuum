using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    //[SerializeField]
    //private Collider2D gravityInfluence;
    [SerializeField]
    private bool inverseSquare = true;

    public float g = 0.5f;

    private List<Entity> entities = new List<Entity>();

    private void FixedUpdate() {
        foreach(Entity entity in entities) {
            if(!entity.grounded) {
                //entity.rb.AddForce((entity.transform.position - transform.position) * -g);
                float force = -g;
                float distance = Vector3.Distance(entity.transform.position, transform.position);
                if(inverseSquare) {
                    force = (-g * ( (Mathf.Pow(transform.localScale.x,2)) / (Mathf.Pow(distance,2) ) ) );
                    entity.rb.AddForce((entity.transform.position - transform.position) * force );
                } else {
                    entity.rb.AddForce((entity.transform.position - transform.position) * force );
                }

                //Debug.Log(entity.gameObject.name + " force: " + force + " | distance: " + distance);
                //Debug.DrawLine(entity.transform.position, entity.transform.position + ((entity.transform.position - transform.position) * force) , Color.red);
                Debug.DrawRay(entity.transform.position, (entity.transform.position - transform.position) * force , Color.blue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Entity entity;
        if(other.gameObject.TryGetComponent<Entity>(out entity)){
            entities.Add(entity);
        } else {
            entity = other.gameObject.GetComponentInParent<Entity>();
            if(entity) {
                entities.Add(entity);
            }
        }

        Debug.Log("Entered influence of " + gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Entity entity;
        // if(other.gameObject.TryGetComponent<Entity>(out entity)){
        //     entities.Remove(entity);
        // }

        entity = entities.Find(element => element.gameObject == other.gameObject);
        entities.Remove(entity);

        Debug.Log("Left influence of " + gameObject.name);
    }
}
