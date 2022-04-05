using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Component responsible for enemy behavior and movement.
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float searchRadius;
    [SerializeField] private float velocity;
    [SerializeField] private float kbForce;
    [SerializeField] private float kbTime = 0.5f;

    private GameObject _player;
    private CharacterController _cc;
    private Animator _anim;
    
    
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    // Keep searching for the player in radius until they are found.
    void FixedUpdate()
    {
        if (_player == null)
        {
            SearchForPlayer();
        }
    }

    // Searches for a the player within a radius, starts moving if one is found.
    private void SearchForPlayer()
    {
        var overlaps = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (var collider in overlaps)
        {
            if (collider.CompareTag("Player"))
            {
                _player = collider.gameObject;
                StartCoroutine(MoveToPlayer());
            }
        }
    }
    
    //Runs when the character enters a collision when moving.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            Hit(hit.transform);
            hit.gameObject.GetComponent<PlayerControls>().Hit(transform);
        }
    }

    // Runs when hitting the player. Triggers a knockback effect.
    public void Hit(Transform other)
    {
        var knockback = (transform.position - other.position).normalized * kbForce;
        StopAllCoroutines();
        StartCoroutine(Knockback(knockback));
    }
    
    // Move to towards the player.
    private IEnumerator MoveToPlayer()
    {
        while (true)
        {
            var forward = _player.transform.position - transform.position;
            forward.y = 0;
            forward = forward.normalized * velocity;

            var moveVector = new Vector3(forward.x, -2f, forward.z);

            _cc.Move(moveVector);
            transform.rotation = Quaternion.LookRotation(forward);

            yield return new WaitForFixedUpdate();
        }
    }

    // Couroutine for knockback.
    private IEnumerator Knockback(Vector3 knockback)
    {
        float timer = 0;
        while (timer < kbTime)
        {
            _cc.Move(knockback);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(MoveToPlayer());
    }
}
