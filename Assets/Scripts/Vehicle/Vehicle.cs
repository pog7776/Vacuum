using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : Controllable
{
    private float strafeMod = 0.5f;

    [SerializeField]
    private float turnSpeed = 1f;

    [SerializeField]
    public ShipResources resources;

    public Transform dismountAnchor;

    [HideInInspector]
    public Station currentStation;

    [SerializeField]
    private float dockedMass = 50f;
    private Dictionary<ModuleType, List<IModule>> modules;
    //private ModuleLoadout moduleLoadout;
    [SerializeField]
    private ModuleCapacityDictionary moduleCapacity;

    #region Events
    public event Action AOnMove;
    #endregion

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

        InitShip();
    }

    private void InitShip() {
        modules = new Dictionary<ModuleType, List<IModule>>();
        foreach (ModuleType type in Enum.GetValues(typeof(ModuleType))) {
            modules[type] = new List<IModule>();
        }
        
        // Install the base engine
        EngineModule baseEngine = new EngineModule(this);
        baseEngine.ThrustOutput = moveSpeed;
        InstallModule(baseEngine);
        //resources[PowerSource.Fuel].Value = resources.fuelCapacity;

        FuelTankModule fuelTank = new FuelTankModule(this);
        fuelTank.Capacity = 100;
        Debug.Log("Before Install: " + resources[PowerSource.Fuel].Capacity);
        InstallModule(fuelTank);
        Debug.Log("After Install: " + resources[PowerSource.Fuel].Capacity);
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
        if(resources[PowerSource.Fuel].Value > 0) {
            //rb.AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * (moveSpeed * direction.y));
            RigidBody.AddForce(transform.up * (MoveSpeed * direction.y));
            RigidBody.AddRelativeForce(new Vector3(direction.x, 0, 0) * (MoveSpeed * strafeMod));

            if(direction != Vector3.zero) {
                AOnMove?.Invoke();
                Debug.Log("Fuel: " + resources[PowerSource.Fuel].Value);
            }
        }
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
        mod = mod == null ? new ThrusterModule(this) : mod;
        mod.ThrustScale = thrust;
        InstallModule(mod);
    }

    public void UntestModule() {
        UninstallModule(mod);
    }

    public void InstallModule(IModule module) {
        ModuleType type = module.ModuleType;

        if(!modules[type].Contains(module)) {
            if(modules[type].Count < moduleCapacity[type]) {
                modules[type].Add(module);
                module.PreInstall(this);
                module.Install(this);
                module.PostInstall(this);
                ReportModuleCapacity(module);
            } else {
                Debug.Log("Already at " + type.ToString() + " capacity. (" + modules[type].Count + " / " + moduleCapacity[type] + ")");
            }
        } else {
            Debug.LogWarning("This module is already installed");
        }
    }

    public void UninstallModule(IModule module) {
        ModuleType type = module.ModuleType;
        if(modules.ContainsKey(type)) {
            if(modules[type].Contains(module)) {
                modules[type].Remove(module);
                module.Uninstall(this);
                ReportModuleCapacity(module);
            } else {
                Debug.LogWarning("This module is not installed");
            }
        }
    }

    private void ReportModuleCapacity(IModule module) {
        if(modules[module.ModuleType].Count < moduleCapacity[module.ModuleType]) {
            Debug.Log(module.ModuleType.ToString() + ": " + modules[module.ModuleType].Count + " / " + moduleCapacity[module.ModuleType] + " installed.");
        }
    }

    public void ConsumeResource(PowerSource resource, float amount){
        resources[resource].Consume(amount);
    }

    public void Refuel(PowerSource resource, float amount) {
        resources[resource].Refuel(amount);
    }

    public void TestRefuel(float amount) {
        Refuel(PowerSource.Fuel, amount);
    }
}

[Serializable]
public class ShipResources {
    [SerializeField]
    public SerializableDictionary<PowerSource, Resource> Resources { get; }

    public ShipResources() {
        Resources = new SerializableDictionary<PowerSource, Resource>();

        foreach(PowerSource sourceType in Enum.GetValues(typeof(PowerSource))) {
            Resources.Add(sourceType, new Resource(sourceType, 0));
        }
    }

    public Resource this[PowerSource key] {
        get => Resources[key];
        set => Resources[key] = value;
    }
}

[Serializable]
public class Resource {
    [SerializeField]
    public PowerSource PowerSource { get; set; }
    [SerializeField]
    public float Capacity { get; set; }
    [SerializeField]
    public float Value { get; set; }

    public Resource(PowerSource resourceType, float capacity) {
        PowerSource = resourceType;
        Capacity = capacity;
        //Refuel(Capacity);
    }

    public void Refuel(float fuelAmount) {
        if(Value + fuelAmount <= Capacity) {
            Value = fuelAmount;
            Debug.Log("Refuled: " + fuelAmount);
        } else {
            float fueledAmount = Capacity - Value;
            Value = Capacity;
            Debug.Log("Refuled: " + fueledAmount);
        }
    }

    public void Consume(float amount) {
        Value -= amount;
    }
}

public enum PowerSource {
    Fuel,
    Energy,
    None
}