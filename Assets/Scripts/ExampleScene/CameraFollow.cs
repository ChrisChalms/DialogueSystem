using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Vector3 _offset;
    [SerializeField]
    private float _smoothSpeed = 0.13f;

    #region MonoBehaviour

    // Move camera
    private void LateUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
        transform.position = smoothedPosition;
    }

    #endregion
}
