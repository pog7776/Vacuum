using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxThrustModule : ThrusterModule {
    public AuxThrustModule(PowerSource source, float consumptionRate) : base(source, consumptionRate) {
        Name = "Thruster";
        ModuleType = ModuleType.Thruster;
    }

    public override void Install(Vehicle vehicle) {
        base.Install(vehicle);
        vehicle.AOnMove += OnUse;
    }

    public override void Uninstall(Vehicle vehicle) {
        base.Uninstall(vehicle);
        vehicle.AOnMove -= OnUse;
    }

    public override void OnUse() {
        base.OnUse();
        if(Vehicle.resources[PowerConsumption.Source].Value > 0) {
            Vehicle.RigidBody.AddForce(Vehicle.transform.up * (ThrustOutput * Vehicle.MoveDirection.y));
            Vehicle.RigidBody.AddRelativeForce(new Vector3(Vehicle.MoveDirection.x, 0, 0) * (ThrustOutput * Vehicle.StrafeMod));
        }
    }
}
