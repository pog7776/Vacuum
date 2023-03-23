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
    protected float _fuelConsumption = 0f;
    public float FuelConsumption { get => _fuelConsumption; set => _fuelConsumption = value; }

    private float originalSpeed;
    private float moddedSpeed;

    public void Install(Vehicle vehicle) {
        originalSpeed = vehicle.moveSpeed;
        vehicle.moveSpeed *= ThrustScale;
        moddedSpeed = vehicle.moveSpeed;
    }

    public void Uninstall(Vehicle vehicle) {
        vehicle.moveSpeed -= moddedSpeed - originalSpeed;
    }
}
