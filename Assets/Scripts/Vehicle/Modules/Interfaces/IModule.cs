using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule : IItem {
    public ModuleType ModuleType { get; set; }
    public PowerConsumption PowerConsumption { get; set; }

    public Vehicle Vehicle { get; set; }

    // TODO Wear over time?
    //public float Age { get; set; }
    public void PreInstall(Vehicle vehicle);
    public void Install(Vehicle vehicle);
    // TODO Not sure if these are needed? ⬇⬇
    public void PostInstall(Vehicle vehicle);
    public void Uninstall(Vehicle vehicle);

    public void OnUse();
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
    private Vehicle _vehicle;
    public Vehicle Vehicle {
        get => _vehicle;
        set {
            if(value == null) {
                _vehicle.AOnMove -= ConsumeResource;
            }
            else if(Source != PowerSource.None) {
                value.AOnMove += ConsumeResource;
            }
            _vehicle = value;
        }
    }

    public PowerConsumption(PowerSource source, float rate) {
        Source = source;
        Rate = rate;
        //this.vehicle = vehicle;

        // if(Source != PowerSource.None) {
        //     Vehicle.AOnMove += ConsumeResource;
        // }
    }

    private void ConsumeResource() {
        Vehicle.ConsumeResource(Source, Rate);
    }
}