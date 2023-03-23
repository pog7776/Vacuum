using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule : IItem {
    public ModuleType ModuleType { get; set; }

    // TODO Wear over time?
    //public float Age { get; set; }
    public void Install(Vehicle vehicle);
    public void Uninstall(Vehicle vehicle);
}

public enum ModuleType {
    Thruster,
    Afterburner,
    RetroThrusters,
    ControlThrusters,
    Shield,
    Weapon,
    Drone,
    Sensors,
    CargoBay
}
