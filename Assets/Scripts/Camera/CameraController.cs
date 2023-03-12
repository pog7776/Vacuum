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

    private Vector3 _offsetPosition;
    public Vector3 OffsetPosition {
        get => _offsetPosition;
        set => _offsetPosition = new Vector3(value.x, value.y, transform.position.z);
    }

    public bool OffsetCamera = false;

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
        LockCamera();
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
            // Vector3 newDir = (newPos - Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).normalized);
            // if(OffsetCamera) {
            //     //Debug.Log(newDir);
            //     newPos = (newPos + (newDir * 5)) / 2;
            // }
            cam.transform.position = newPos;
        }
    }
}
