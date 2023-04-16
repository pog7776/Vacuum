using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterBurnerModule : ThrusterModule {
    public AfterBurnerModule(PowerSource source, float consumptionRate) : base(source, consumptionRate) {
        Name = "AfterBurner";
        ModuleType = ModuleType.Afterburner;
    }

    public override void Install(Vehicle vehicle) {
        base.Install(vehicle);
        Vehicle.AOnAfterBurner += OnUse;
    }

    public override void Uninstall(Vehicle vehicle) {
        base.Uninstall(vehicle);
        vehicle.AOnAfterBurner -= OnUse;
    }

    public override void OnUse() {
        base.OnUse();
        if(Vehicle.resources[PowerConsumption.Source].Value > 0) {
            Vehicle.RigidBody.AddForce(Vehicle.transform.up * (ThrustOutput));
            PowerConsumption.ConsumeResource(Vehicle);
        }
    }
}
