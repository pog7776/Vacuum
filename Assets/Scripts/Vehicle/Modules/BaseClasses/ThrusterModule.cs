using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterModule : IModule
{
    protected string _name = "Thruster";
    public string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType = ModuleType.Thruster;
    public ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    protected float _baseCost = 0f;
    public float BaseCost { get => _baseCost; set => _baseCost = value; }
    protected float _thrustScale = 1f;
    public float ThrustScale { get => _thrustScale; set => _thrustScale = value; }
    protected float _fuelConsumption = 1f;
    public float FuelConsumption { get => _fuelConsumption; set => _fuelConsumption = value; }
    public PowerConsumption PowerConsumption { get; set; }

    private float originalSpeed;
    private float moddedSpeed;

    public ThrusterModule(Vehicle vehicle) {
        PowerConsumption = new PowerConsumption(PowerSource.Fuel, 0.05f, vehicle);
    }

    public void Install(Vehicle vehicle) {
        originalSpeed = vehicle.moveSpeedMod;
        vehicle.moveSpeedMod += ThrustScale;
        moddedSpeed = vehicle.moveSpeedMod;
    }

    public void PreInstall(Vehicle vehicle) {

    }

    public void PostInstall(Vehicle vehicle) {
        
    }

    public void Uninstall(Vehicle vehicle) {
        vehicle.moveSpeedMod -= moddedSpeed - originalSpeed;
    }
}
