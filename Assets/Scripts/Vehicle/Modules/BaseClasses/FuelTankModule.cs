using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTankModule : IModule
{
    protected string _name = "FuelTank";
    public string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType = ModuleType.FuelTank;
    public ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    protected float _baseCost = 0f;
    public float BaseCost { get => _baseCost; set => _baseCost = value; }
    public PowerConsumption PowerConsumption { get; set; }

    public float Capacity = 100;

    private float originalTankSize;
    private float moddedTankSize;

    public FuelTankModule(Vehicle vehicle) {
        PowerConsumption = new PowerConsumption(PowerSource.None, 0, vehicle);
    }

    public void Install(Vehicle vehicle) {
        originalTankSize = vehicle.resources[PowerSource.Fuel].Capacity;
        vehicle.resources[PowerSource.Fuel].Capacity = Capacity;
        moddedTankSize = vehicle.resources[PowerSource.Fuel].Capacity;
    }

    public void PreInstall(Vehicle vehicle) {
        
    }

    public void PostInstall(Vehicle vehicle) {

    }

    public void Uninstall(Vehicle vehicle) {
        vehicle.resources[PowerSource.Fuel].Capacity -= moddedTankSize - originalTankSize;
    }
}
