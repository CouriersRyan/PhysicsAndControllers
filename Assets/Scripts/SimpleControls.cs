using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleControls : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    
    private Vector3 _moveVec;
    private bool _isJump;
    private bool _isGrounded;

    private Vector2 _prevMoveVec;

    [SerializeField] private float gravity = 2f;
    [SerializeField] private float moveSpd = 0.3f;
    [SerializeField] private float jumpStrength = 1f;
    
    
    private static readonly int Jump = Animator.StringToHash("OnJump");
    private static readonly int Grounded = Animator.StringToHash("IsGrounded");
    private static readonly int Velocity = Animator.StringToHash("Velocity");

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        _isGrounded = _cc.isGrounded;
        _anim.SetBool(Grounded, _isGrounded);

        if (!_isGrounded)
        {
            if (_moveVec.y < 0) _isJump = false;
        } else
        {
            _anim.SetFloat(Velocity, _moveVec.magnitude);
            if (!_isJump)
            {
                _moveVec.y = 0;
            }
        }
        
        _moveVec.y -= gravity * Time.deltaTime;
        _cc.Move(_moveVec);
        
        var forward = new Vector3(_moveVec.x, 0, _moveVec.z);
        if(forward.magnitude != 0)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
    

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        _prevMoveVec = inputVec;
        Move(inputVec);
    }

    //Set a new move vector based on the passed in vector2
    private void Move(Vector2 inputVec)
    {
        var turnedVec = AdjustFacing(inputVec);

        Vector3 temp = new Vector3(turnedVec.x, 0, turnedVec.y) * moveSpd;

        _moveVec.x = temp.x;
        _moveVec.z = temp.z;
    }

    //Rotates the input based on the direction the player is facing.
    private Vector3 AdjustFacing(Vector2 inputVec)
    {
        var facing = Camera.main.transform.eulerAngles.y;

        var turnedVec = Quaternion.Euler(0, 0, -facing) * inputVec;
        return turnedVec;
    }

    public void OnRotateCamera(InputValue value)
    {
        Move(_prevMoveVec);
    }

    public void OnJump(InputValue input)
    {
        if (_cc.isGrounded)
        {
            _moveVec.y = jumpStrength;
            _isJump = true;
            _anim.SetTrigger(Jump);
        }
    }
}
