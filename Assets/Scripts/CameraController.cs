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
    
    // Get the initial position of the camera relative to the target and uses that as the default posiiton and rotation.
    void Start()
    {
        _cameraBoom = transform.position - targetGameObject.transform.position;
        _cameraBoomRotation = transform.rotation;
    }

    // Adjusts the camera position based on the player's position and the rotation input.
    void LateUpdate()
    {
        transform.position = targetGameObject.transform.position + _cameraRotation * _cameraBoom;
        transform.rotation = _cameraRotation * _cameraBoomRotation;
    }

    // Takes in a vector2 input and converts it into a Quaternion for the camera to rotate on.
    public void OnRotateCamera(InputValue input)
    {
        var vec = input.Get<Vector2>();
        _rotateX += vec.x * sensitivity;
        _rotateY -= vec.y * sensitivity;
        _rotateY = Mathf.Clamp(_rotateY, clampYBot, clampYTop);
        _cameraRotation = Quaternion.Euler(_rotateY, _rotateX, 0);
    }
}
