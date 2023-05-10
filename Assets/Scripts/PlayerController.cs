using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touchingDirections;

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private float speed = 750.0f;
    [SerializeField] private float jumpForce = 2000.0f;
    [SerializeField] private float crouchThreshold = -0.5f;
    [SerializeField] private float walkThreshold = 0.5f;

    [SerializeField] private bool _isWalking = false;
    public bool IsWalking
    {
        get
        {
            return _isWalking;
        }
        private set
        {
            _isWalking = value;
            animator.SetBool(AnimationStrings.isWalking, value);
        }
    }

    [SerializeField] private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);

                if (rb.velocity.y == 0 && !IsCrouched)
                    animator.SetTrigger(AnimationStrings.triggerTurn);
            }

            _isFacingRight = value;
            animator.SetBool(AnimationStrings.isFacingRight, value);
        }
    }

    [SerializeField] private bool _isCrouched = true;
    public bool IsCrouched
    {
        get
        {
            return _isCrouched;
        }
        private set
        {
            _isCrouched = value;
            animator.SetBool(AnimationStrings.isCrouched, value);
        }
    }

    public bool CanWalk
    {
        get
        {
            return animator.GetBool(AnimationStrings.canWalk);
        }
    }

    public bool CanJump
    {
        get
        {
            return animator.GetBool(AnimationStrings.canJump);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if (IsWalking && CanWalk)
        {
            rb.velocity = new Vector2(moveInput.x * speed * Time.deltaTime, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }


        animator.SetFloat(AnimationStrings.velocityY, rb.velocity.y);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > walkThreshold && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < -walkThreshold && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Velocity updated in FixedUpdate
        moveInput = context.ReadValue<Vector2>();
        SetFacingDirection(moveInput);
        IsWalking = Mathf.Abs(moveInput.x) > walkThreshold;

        IsCrouched = moveInput.y < crouchThreshold;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            animator.SetTrigger(AnimationStrings.triggerJump);
        }
    }
}
