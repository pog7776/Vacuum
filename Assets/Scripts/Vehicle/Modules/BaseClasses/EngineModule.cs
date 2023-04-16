using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineModule : ThrusterModule {
    public EngineModule(PowerSource source, float consumptionRate) : base(source, consumptionRate) {
        Name = "Engine";
        ModuleType = ModuleType.Engine;
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
            //Vehicle.RigidBody.AddRelativeForce(new Vector3(Vehicle.MoveDirection.x, 0, 0) * (ThrustOutput * Vehicle.StrafeMod));
        }
    }
}
