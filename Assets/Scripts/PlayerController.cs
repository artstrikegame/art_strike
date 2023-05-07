using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private TouchingDirections touchingDirections;

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private float speed = 15.0f;
    [SerializeField] private float jumpForce = 40.0f;
    [SerializeField] private float crouchTreshold = -0.5f;


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

                if (rigidBody2D.velocity.y == 0 && !IsCrouched)
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

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        rigidBody2D.velocity = new Vector2(moveInput.x * speed, rigidBody2D.velocity.y);
        animator.SetFloat(AnimationStrings.velocityY, rigidBody2D.velocity.y);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Movement updated in FixedUpdate
        moveInput = context.ReadValue<Vector2>();
        SetFacingDirection(moveInput);
        IsWalking = moveInput.x != 0;

        IsCrouched = moveInput.y < crouchTreshold;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
            animator.SetTrigger(AnimationStrings.triggerJump);
        }
    }
}
