using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private float _targetSize = 10;
    public float TargetSize {
        get => _targetSize;
        set => _targetSize = Mathf.Clamp(value, MinSize, MaxSize);
    }
    
    private float _minSize = 10;
    public float MinSize {
        get => _minSize;
        set => _minSize = value;
    }

    private float _maxSize = 100;
    public float MaxSize {
        get => _maxSize;
        set => _maxSize = value;
    }

    private float _zoomSpeed = 1;
    public float ZoomSpeed {
        get => _zoomSpeed;
        set => _zoomSpeed = value;
    }

    private GameObject _followTarget;
    public GameObject FollowTarget {
        get => _followTarget;
        set => _followTarget = value;
    }

    public bool offsetCamera = false;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate() // late or fixed update?
    {
        // TODO Can put a bool that is set when target size changes so this isn't always called
        LerpToSize();
        if(!offsetCamera) {
            LockCamera();
        }
    }

    private void FixedUpdate() {
        if(offsetCamera) {
            LockCamera();
        }
    }

    private void LerpToSize() {
        if(cam != null) {
            cam.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, TargetSize, Time.deltaTime * ZoomSpeed);
        }
    }

    private void LockCamera()
    {
        // Implement lerp or bool to toggle lerp to target?
        if(FollowTarget) {
            Vector3 newPos = new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y, cam.transform.position.z);

            if(offsetCamera) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3 dir = (newPos - mousePos).normalized;
                Vector2 camCenter = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
                float power = Vector2.Distance(Mouse.current.position.ReadValue(), camCenter);
                newPos -= Vector3.ClampMagnitude(dir * power/100, 5);
                //cam.transform.position = Vector3.Slerp(cam.transform.position, -newPos, Time.fixedDeltaTime);
            }

            cam.transform.position = newPos;
        }
    }
}
