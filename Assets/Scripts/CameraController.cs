using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject targetGameObject;

    private Vector3 _cameraBoom;
    private Quaternion _cameraBoomRotation;
    private float _rotateX;
    private float _rotateY;
    [SerializeField] private float sensitivity;

    [SerializeField] private float clampYTop = 15f;
    [SerializeField] private float clampYBot = -15f;

    private Quaternion _cameraRotation;

    public Quaternion CameraRotation
    {
        private set;
        get;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraBoom = transform.position - targetGameObject.transform.position;
        _cameraBoomRotation = transform.rotation;
        var simpleControls = targetGameObject.GetComponent<SimpleControls>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = targetGameObject.transform.position + _cameraRotation * _cameraBoom;
        transform.rotation = _cameraRotation * _cameraBoomRotation;
    }

    public void OnRotateCamera(InputValue input)
    {
        var vec = input.Get<Vector2>();
        _rotateX += vec.x * sensitivity;
        _rotateY -= vec.y * sensitivity;
        _rotateY = Mathf.Clamp(_rotateY, clampYBot, clampYTop);
        _cameraRotation = Quaternion.Euler(_rotateY, _rotateX, 0);
    }
}
