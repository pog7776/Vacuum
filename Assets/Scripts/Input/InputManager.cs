using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour {
    private Dictionary<KeyCode, UnityEvent> inputActions;
    // Start is called before the first frame update
    void Start() {
        inputActions = new Dictionary<KeyCode, UnityEvent>();
    }

    // Update is called once per frame
    void Update() {
        if(Input.anyKey) {
            foreach(KeyValuePair<KeyCode, UnityEvent> action in inputActions) {
                action.Value?.Invoke();
            }
        }
    }

    public void RegisterAction(KeyCode keyCode, UnityAction inputEvent) {
        if(!inputActions.ContainsKey(keyCode)) {
            inputActions.Add(keyCode, new UnityEvent());
        }
        inputActions[keyCode].AddListener(inputEvent);
    }

    public void UnregisterAction(KeyCode keyCode, UnityAction inputEvent) {
        if(inputActions.ContainsKey(keyCode)) {
            inputActions[keyCode].RemoveListener(inputEvent);
        }
    }
}
