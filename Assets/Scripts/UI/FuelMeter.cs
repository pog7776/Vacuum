using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelMeter : MonoBehaviour {
    [SerializeField]
    private Vehicle _currentVehicle;
    public Vehicle CurrentVehicle {
        get {
            return _currentVehicle;
        }
        set {
            _currentVehicle = value;
            gauge.enabled = true;
        }
    }

    private Slider gauge;

    public PowerSource resourceType = PowerSource.Fuel;

    // Start is called before the first frame update
    void Start() {
        gauge = GetComponent<Slider>();
        if(!CurrentVehicle) {
            gauge.enabled = false;
        } else {
            CurrentVehicle.resources[resourceType].AOnUpdateResource += UpdateSlider;
        }
    }

    private void UpdateSlider() {
        gauge.value = CurrentVehicle.resources[resourceType].Value / CurrentVehicle.resources[resourceType].Capacity;

        //gauge.value = Mathf.Lerp(gauge.value, CurrentVehicle.resources[resourceType].Value / CurrentVehicle.resources[resourceType].Capacity, Time.deltaTime);
    }
}
