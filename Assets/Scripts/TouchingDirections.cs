using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    Animator animator;
    CapsuleCollider2D touchingCollider;

    public ContactFilter2D castFilter;
    public float groundDistance = .05f;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];

    [SerializeField] private bool _isGrounded = true;
    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        touchingCollider = GetComponent<CapsuleCollider2D>();
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        IsGrounded = touchingCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
    }
}
