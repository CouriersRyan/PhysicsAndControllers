using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    
    private Vector3 _moveVec;
    private bool _isJump;
    private bool _isGrounded;
    private bool _isKB;

    private Vector2 _prevMoveVec;

    [SerializeField] private float gravity = 2f;
    [SerializeField] private float moveSpd = 0.3f;
    [SerializeField] private float jumpStrength = 1f;
    [SerializeField] private float kbTime;
    [SerializeField] private float kbForce;
    
    
    private static readonly int Jump = Animator.StringToHash("OnJump");
    private static readonly int Grounded = Animator.StringToHash("IsGrounded");
    private static readonly int Velocity = Animator.StringToHash("Velocity");

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _cc.detectCollisions = true;
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

        if (!_isKB)
        {
            _moveVec.y -= gravity * Time.deltaTime;
            _cc.Move(_moveVec);
        
            var forward = new Vector3(_moveVec.x, 0, _moveVec.z);
            if(forward.magnitude != 0)
            {
                transform.rotation = Quaternion.LookRotation(forward);
            }
        }
    }
    
    //When a move input is pressed, set the horizontal direction and speed of movement.
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

    // Run move again when the camera changes with old move vector. This is so the player can move in a direction based on the camera.
    public void OnRotateCamera(InputValue value)
    {
        Move(_prevMoveVec);
    }

    // On the jump input, if the player is grounded, then jump.
    public void OnJump(InputValue input)
    {
        if (_cc.isGrounded)
        {
            _moveVec.y = jumpStrength;
            _isJump = true;
            _anim.SetTrigger(Jump);
        }
    }

    // Runs when characters enters a collision while moving.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            _isKB = true;
            Hit(hit.transform);
            hit.gameObject.GetComponent<EnemyMove>().Hit(transform);
        }
    }
    
    // Runs when hitting an enemy. Triggers a knockback effect.
    public void Hit(Transform other)
    {
        var knockback = (transform.position - other.position).normalized * kbForce;
        StopAllCoroutines();
        StartCoroutine(Knockback(knockback));
    }
    
    // Couroutine for the knockback.
    private IEnumerator Knockback(Vector3 knockback)
    {
        float timer = 0;
        while (timer < kbTime)
        {
            _cc.Move(knockback);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isKB = false;
    }
}