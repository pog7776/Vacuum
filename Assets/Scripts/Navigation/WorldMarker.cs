using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMarker : MonoBehaviour
{
    [SerializeField]
    private string MarkerName = "New Marker";

    [SerializeField]
    private MarkerType markerType = MarkerType.Unknown;

    [SerializeField]
    private Threat threat = Threat.Unknown;

    [SerializeField]
    private Color markerColor = Color.white;

    [SerializeField]
    private float beaconRange = 100.0f;

    [SerializeField]
    private float beaconInnerRange = 0.0f;

    [SerializeField]
    private bool drawRadius = false;

    [SerializeField]
    private LineRenderer line;

    private PlayerController player;

    // Start is called before the first frame update
    void Start() {
        //TODO Probably replace finding the player with a navigation controller
        // then I can register beacons on that, and will be usable with a map
        // TODO Add a sprite field for a map icon
        player = FindObjectOfType<PlayerController>();

        if(!TryGetComponent<LineRenderer>(out line))
        {
            line = gameObject.AddComponent<LineRenderer>();
        }
        line.positionCount = 2;
        line.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if(player) {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if(distance < beaconRange && distance > beaconInnerRange) {
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, player.transform.position);
            } else {
                line.enabled = false;
            }
        }
    }

    private void OnDrawGizmos() {
        if(drawRadius) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, beaconRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, beaconInnerRange);
        }
    }
}

enum MarkerType {
    Station,
    Ship,
    Planet,
    Moon,
    Asteroid,
    BlackHole,
    Unknown
}

enum Threat {
    Safe,
    Warning,
    Dangerous,
    Hostile,
    Unknown
}
