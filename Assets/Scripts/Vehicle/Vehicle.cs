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

    [SerializeField]
    private float dockedMass = 50f;

    private Dictionary<ModuleType, List<IModule>> modules;

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

        modules = new Dictionary<ModuleType, List<IModule>>();
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

    public override void Posess() {
        base.Posess();
        RigidBody.mass = 1;
    }

    public override void UnPosess() {
        base.UnPosess();
        if(currentStation) {
            RigidBody.mass = dockedMass;
        }
    }

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

            if(!posessed) {
                RigidBody.mass = dockedMass;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Gravity") {
            if(currentStation != null) {
                currentStation = null;
            }

            RigidBody.mass = 1;
        }
    }

    private ThrusterModule mod;
    public void TestModule(float thrust) {
        mod = mod == null ? new ThrusterModule() : mod;
        mod.ThrustScale = thrust;
        InstallModule(mod);
    }

    public void UntestModule() {
        UninstallModule(mod);
    }

    public void InstallModule(IModule module) {
        if(!modules.ContainsKey(module.ModuleType)) {
            modules.Add(module.ModuleType, new List<IModule>());
        }

        if(!modules[module.ModuleType].Contains(module)) {
            modules[module.ModuleType].Add(module);
            module.Install(this);
        } else {
            Debug.LogWarning("This module is already installed");
        }

        // switch (module.ModuleType)
        // {
        //     case ModuleType.Thruster:
        //         ThrusterModule mod = module as ThrusterModule;
        //         moveSpeed *= mod.ThrustScale;
        //         Debug.Log("Installed thruster module: " + mod.Name);
        //         break;
        //     default:
        //         break;
        // }
    }

    public void UninstallModule(IModule module) {
        if(modules.ContainsKey(module.ModuleType)) {
            if(modules[module.ModuleType].Contains(module)) {
                modules[module.ModuleType].Remove(module);
                module.Uninstall(this);
            } else {
                Debug.LogWarning("This module is not installed");
            }
        }

        // switch (module.ModuleType)
        // {
        //     case ModuleType.Thruster:
        //         Debug.Log("AAAAAA");
        //         break;
        //     default:
        //         break;
        // }
    }
}
