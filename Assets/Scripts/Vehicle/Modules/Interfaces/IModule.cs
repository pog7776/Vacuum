using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule : IItem {
    public ModuleType ModuleType { get; set; }
    public PowerConsumption PowerConsumption { get; set; }

    // TODO Wear over time?
    //public float Age { get; set; }
    public void PreInstall(Vehicle vehicle);
    public void Install(Vehicle vehicle);
    // TODO Not sure if these are needed? ⬇⬇
    public void PostInstall(Vehicle vehicle);
    public void Uninstall(Vehicle vehicle);
}

public enum ModuleType {
    Engine,
    Thruster,
    Afterburner,
    RetroThrusters,
    ControlThrusters,
    Shield,
    Weapon,
    Drone,
    Sensors,
    CargoBay,
    FuelTank,
    Battery
}

public class PowerConsumption {
    // Needs to be per second.
    // Power usage needs to be handled by each module
    // That way, each module only consumes resource while it should
    // Perhaps a resource class in the vehicle, so module can reference a resource by key
    // Then have the resource subscribe to an event or action in the vehicle class that uses a resource
    public PowerSource Source { get; set; }
    public float Rate { get; set; }
    private Vehicle vehicle;

    public PowerConsumption(PowerSource source, float rate, Vehicle vehicle) {
        Source = source;
        Rate = rate;
        this.vehicle = vehicle;

        if(Source != PowerSource.None) {
            vehicle.AOnMove += ConsumeResource;
        }
    }

    private void ConsumeResource() {
        vehicle.ConsumeResource(Source, Rate);
    }
}