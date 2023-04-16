using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle : Controllable
{
    private float _strafeMod = 0.5f;
    public float StrafeMod {
        get => _strafeMod;
        set => _strafeMod = value;
    }

    [SerializeField]
    private float turnSpeed = 1f;
    private bool afterBurnerEngaged = false;
    [SerializeField]
    private float fuelCapacity = 100;
    [SerializeField]
    private float batteryCapacity = 100;

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
    public event Action AOnAfterBurner;
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
        EngineModule baseEngine = new EngineModule(PowerSource.Fuel, 0.1f);
        baseEngine.ThrustOutput = moveSpeed;
        InstallModule(baseEngine);

        ResourceTankModule fuelTank = new ResourceTankModule(ModuleType.FuelTank, PowerSource.Fuel, this);
        fuelTank.Capacity = fuelCapacity;
        InstallModule(fuelTank);

        ResourceTankModule battery = new ResourceTankModule(ModuleType.Battery, PowerSource.Energy, this);
        battery.Capacity = batteryCapacity;
        InstallModule(battery);

        AuxThrustModule ionEngine = new AuxThrustModule(PowerSource.Energy, 0.05f);
        ionEngine.ThrustOutput = 5;
        ionEngine.PowerConsumption.Source = PowerSource.Energy;
        ionEngine.PowerConsumption.Rate = 0.05f;
        InstallModule(ionEngine);

        AfterBurnerModule afterBurner = new AfterBurnerModule(PowerSource.Fuel, 0.5f);
        afterBurner.ThrustOutput = 40;
        InstallModule(afterBurner);
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
        if(afterBurnerEngaged) {
            AOnAfterBurner?.Invoke();
        }
    }

    protected override void Move(Vector3 direction) {
        if(direction != Vector3.zero) {
            AOnMove?.Invoke();
        }
    }

    public void OnAfterBurner(InputValue value) {
        if(value.Get<float>() == 1) {
            afterBurnerEngaged = true;
        } else {
            afterBurnerEngaged = false;
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
        mod = mod == null ? new AuxThrustModule(PowerSource.Fuel, 0.05f) : mod;
        mod.ThrustOutput = thrust;
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
        Refuel(PowerSource.Energy, amount);
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

    public event Action AOnUpdateResource;

    public Resource(PowerSource resourceType, float capacity) {
        PowerSource = resourceType;
        Capacity = capacity;
    }

    public void Refuel(float fuelAmount) {
        if(Value + fuelAmount <= Capacity) {
            Value += fuelAmount;
            Debug.Log("Refuled: " + fuelAmount);
        } else {
            float fueledAmount = Capacity - Value;
            Value = Capacity;
            Debug.Log("Refuled: " + fueledAmount);
        }
        AOnUpdateResource?.Invoke();
    }

    public void Consume(float amount) {
        Value -= amount;
        AOnUpdateResource?.Invoke();
    }
}

public enum PowerSource {
    Fuel,
    Energy,
    None
}