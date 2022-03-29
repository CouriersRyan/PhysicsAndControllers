using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleControls : MonoBehaviour
{
    private CharacterController _cc;
    
    private Vector3 _moveVec;
    [SerializeField] private bool _isJump;
    [SerializeField] private bool isGrounded;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        isGrounded = _cc.isGrounded;

        if (!_cc.isGrounded)
        {
            if (_moveVec.y < 0) _isJump = false;
        } else
        {
            if (!_isJump)
            {
                _moveVec.y = 0;
            }
        }
        
        _moveVec.y -= 4.5f * Time.deltaTime;
        _cc.Move(_moveVec);

        transform.rotation = Quaternion.LookRotation(new Vector3(_moveVec.x, 0, _moveVec.z));
    }
    

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        
        var temp = new Vector3(inputVec.x, 0, inputVec.y) * 0.3f;

        _moveVec.x = temp.x;
        _moveVec.z = temp.z;
    }

    public void OnJump(InputValue input)
    {
        if (_cc.isGrounded)
        {
            _moveVec.y = 1f;
            _isJump = true;
        }
    }
}
