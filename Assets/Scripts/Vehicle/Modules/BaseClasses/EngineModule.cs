using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineModule : IModule
{
    protected string _name = "Engine";
    public string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType = ModuleType.Engine;
    public ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    private Vehicle _vehicle;
    public Vehicle Vehicle {
        get => _vehicle;
        set {
            _vehicle = value;
            PowerConsumption.Vehicle = value;
        }
    }
    protected float _baseCost = 0f;
    public float BaseCost { get => _baseCost; set => _baseCost = value; }
    protected float _thrustOutput = 1f;
    public float ThrustOutput { get => _thrustOutput; set => _thrustOutput = value; }
    protected float _fuelConsumption = 0f;
    public float FuelConsumption { get => _fuelConsumption; set => _fuelConsumption = value; }
    public PowerConsumption PowerConsumption { get; set; }

    public EngineModule(Vehicle vehicle, PowerSource source, float consumptionRate) {
        PowerConsumption = new PowerConsumption(source, consumptionRate);
    }

    public void Install(Vehicle vehicle) {
        Vehicle = vehicle;
        vehicle.AOnMove += OnUse;
    }

    public void PreInstall(Vehicle vehicle) {
        
    }

    public void PostInstall(Vehicle vehicle) {

    }

    public void Uninstall(Vehicle vehicle) {
        Vehicle = null;
        vehicle.AOnMove -= OnUse;
    }

    public void OnUse() {
        if(Vehicle.resources[PowerConsumption.Source].Value > 0) {
            Vehicle.RigidBody.AddForce(Vehicle.transform.up * (ThrustOutput * Vehicle.MoveDirection.y));
            Vehicle.RigidBody.AddRelativeForce(new Vector3(Vehicle.MoveDirection.x, 0, 0) * (ThrustOutput * Vehicle.StrafeMod));
        }
    }
}
