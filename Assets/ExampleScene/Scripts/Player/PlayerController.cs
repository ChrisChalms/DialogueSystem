﻿#pragma warning disable 649

using UnityEngine;
using CC.DialogueSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _airMovementSpeed;
    [SerializeField]
    private float _movementSlowdownMultiplier;

    [Header("Jumping")]
    [SerializeField]
    private float _jumpForce;

    private Rigidbody2D _rb;
    private PlayerAnimation _animation;
    private PlayerCollision _collision;

    private bool _canMove;
    private float _inputAxis;
    private int _facingDirection; // 1 = Right, -1 = Left
    private string _hittingCharacter;
    private bool _pressingJump;

    #region MonoBehaviour
    
    // Intialize
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animation = GetComponentInChildren<PlayerAnimation>();
        _collision = GetComponent<PlayerCollision>();

        _canMove = true;
        _facingDirection = 1;

        // Actions
        DialogueController.Instance.ConversationStarted += () =>_canMove = false;
        DialogueController.Instance.ConversationEnded += () => _canMove = true;
    }

    // Input loop
    private void Update()
    {
        _inputAxis = Input.GetAxisRaw("Horizontal");

        // Flip the sprite if we need to
        if (_canMove && (_inputAxis > 0 && _facingDirection == -1) || (_inputAxis < 0 && _facingDirection == 1))
            changeFacingDirection();

        _animation.UpdateVelocity(_rb.velocity);

        checkInputs();
    }

    // Move the player
    private void FixedUpdate()
    {
        if (!_canMove)
            return;

        // Side movement
        if(_inputAxis != 0)
            _rb.velocity = new Vector2(_inputAxis * (_collision.IsGrounded ? _movementSpeed : _airMovementSpeed), _rb.velocity.y);
        else
            _rb.velocity = new Vector2(_rb.velocity.x * _movementSlowdownMultiplier, _rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Character
        if(collider.gameObject.layer == 8)
            _hittingCharacter = collider.gameObject.name;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // Character
        if (collider.gameObject.layer == 8)
            _hittingCharacter = string.Empty;
    }

    #endregion
    
    // Check the user inputs
    private void checkInputs()
    {
        _pressingJump = Input.GetKeyDown(KeyCode.Space);
        if (_pressingJump)
            jump();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(_hittingCharacter))
                DialogueController.Instance.StartConversation(_hittingCharacter);
        }
    }

    // Jump if we can
    private void jump()
    {
        if (_collision.IsGrounded)
        {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            if(VariableRepo.Instance.Retrieve<bool>("trackingJumps"))
            {
                var tempJumps = VariableRepo.Instance.Retrieve<int>("playerJumps");
                tempJumps++;
                VariableRepo.Instance.Register<int>("playerJumps", tempJumps);
            }
        }
    }

    #region Helpers

    // Change the facing direction of the sprite
    private void changeFacingDirection()
    {
        _facingDirection *= -1;
        _animation.Flip(_facingDirection);
    }

    #endregion
}
