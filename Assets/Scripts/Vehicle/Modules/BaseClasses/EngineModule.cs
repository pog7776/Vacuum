using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineModule : IModule
{
    protected string _name = "Engine";
    public string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType = ModuleType.Engine;
    public ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    protected float _baseCost = 0f;
    public float BaseCost { get => _baseCost; set => _baseCost = value; }
    protected float _thrustOutput = 1f;
    public float ThrustOutput { get => _thrustOutput; set => _thrustOutput = value; }
    // TODO make a base "thruster" class or interface that handles fuel
    // That way I can iterate over all thrust module types to figure out fuel consumption
    // Interface might work better so I can include things like fuel and power usage
    // Or just a "thrust" interface that contains most of this functionality
    protected float _fuelConsumption = 0f;
    public float FuelConsumption { get => _fuelConsumption; set => _fuelConsumption = value; }

    public PowerConsumption PowerConsumption { get; set; }

    private float originalSpeed;
    private float moddedSpeed;

    public EngineModule(Vehicle vehicle) {
        PowerConsumption = new PowerConsumption(PowerSource.Fuel, 0.1f, vehicle);
    }

    public void Install(Vehicle vehicle) {
        originalSpeed = vehicle.moveSpeed;
        vehicle.moveSpeed = ThrustOutput;
        moddedSpeed = vehicle.moveSpeed;
    }

    public void PreInstall(Vehicle vehicle) {
        
    }

    public void PostInstall(Vehicle vehicle) {

    }

    public void Uninstall(Vehicle vehicle) {
        vehicle.moveSpeed -= moddedSpeed - originalSpeed;
    }
}
