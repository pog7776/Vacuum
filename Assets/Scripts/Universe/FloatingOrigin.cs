using UnityEngine;

namespace Universe
{
public class FloatingOrigin : MonoBehaviour
{
    [SerializeField]
    private Transform m_referenceTransform;

    [SerializeField]
    private float m_resetMagnitudeLimit = 10000f;

    private Vector3 _previousPosition = Vector3.zero;

    private Vector3 CurrentPosition => m_referenceTransform.position;

    private void Awake() => UpdatePosition();

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // Vector3 currentPosition = CurrentPosition;
        //
        // Vector3 newPosition = transform.position;
        //
        //
        // Vector3 positionDelta = GetPositionDelta(currentPosition, _previousPosition);
        // newPosition -= positionDelta;
        //
        // if(newPosition.magnitude > 100000) { return; }
        //
        // transform.position = newPosition;
        //
        // _previousPosition = positionDelta - currentPosition;
        //transform.position = -CurrentPosition;
        transform.position -= CurrentPosition;

        if(transform.position.magnitude > m_resetMagnitudeLimit)
        {
            for(int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                transform.GetChild(childIndex).localPosition += transform.position;
            }

            transform.position = Vector3.zero;
        }
    }

    private static Vector3 GetPositionDelta(Vector3 currentPosition, Vector3 previousPosition) => currentPosition - previousPosition;
}
}
