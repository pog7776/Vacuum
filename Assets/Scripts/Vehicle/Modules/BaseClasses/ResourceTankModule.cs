using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTankModule : IModule
{
    protected string _name = "ResourceTank";
    public string Name { get => _name; set => _name = value; }
    protected ModuleType _moduleType;
    public ModuleType ModuleType { get => _moduleType; set => _moduleType = value; }
    public Vehicle Vehicle { get; set; }
    protected float _baseCost = 0f;
    public float BaseCost { get => _baseCost; set => _baseCost = value; }
    public PowerConsumption PowerConsumption { get; set; }
    public PowerSource ResourceType { get; set; }

    public float Capacity = 100;

    private float originalTankSize;
    private float moddedTankSize;

    public ResourceTankModule(ModuleType moduleType, PowerSource resourceType, Vehicle vehicle) {
        Vehicle = vehicle;
        ModuleType = moduleType;
        ResourceType = resourceType;
        PowerConsumption = new PowerConsumption(ResourceType, 0);
    }

    public void Install(Vehicle vehicle) {
        Vehicle = vehicle;
        originalTankSize = vehicle.resources[ResourceType].Capacity;
        vehicle.resources[ResourceType].Capacity = Capacity;
        moddedTankSize = vehicle.resources[ResourceType].Capacity;
    }

    public void PreInstall(Vehicle vehicle) {
        
    }

    public void PostInstall(Vehicle vehicle) {

    }

    public void Uninstall(Vehicle vehicle) {
        vehicle.resources[PowerSource.Fuel].Capacity -= moddedTankSize - originalTankSize;
    }

    public void OnUse() {
        
    }
}
