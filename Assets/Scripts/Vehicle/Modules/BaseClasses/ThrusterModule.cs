using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrusterModule : IModule
{
    protected string _name = "Thruster";
    public virtual string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType = ModuleType.Thruster;
    public virtual ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    private Vehicle _vehicle;
    public virtual Vehicle Vehicle {
        get => _vehicle;
        set {
            _vehicle = value;
        }
    }
    protected float _baseCost = 0f;
    public virtual float BaseCost { get => _baseCost; set => _baseCost = value; }
    protected float _thrustScale = 1f;
    public virtual float ThrustOutput { get => _thrustScale; set => _thrustScale = value; }
    protected float _fuelConsumption = 0f;
    public virtual float FuelConsumption { get => _fuelConsumption; set => _fuelConsumption = value; }
    public virtual PowerConsumption PowerConsumption { get; set; }

    public ThrusterModule(PowerSource source, float consumptionRate) {
        PowerConsumption = new PowerConsumption(source, consumptionRate);
    }

    public virtual void Install(Vehicle vehicle) {
        Vehicle = vehicle;
    }

    public virtual void PreInstall(Vehicle vehicle) {

    }

    public virtual void PostInstall(Vehicle vehicle) {
        
    }

    public virtual void Uninstall(Vehicle vehicle) {
        Vehicle = null;
    }

    public virtual void OnUse() {
        if(Vehicle.resources[PowerConsumption.Source].Value > 0) {
            PowerConsumption.ConsumeResource(Vehicle);
        }
    }
}
