using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private DialogueController _dialogueController; // Maybe it should be a singleton?

    private bool _canMove;
    private float _inputAxis;
    private float _movementSpeed;
    private string _hittingCharacter;

    #region MonoBehaviour
    
    // Intialize
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _dialogueController = GameObject.Find("DialogueController").GetComponent<DialogueController>();

        _canMove = true;
        _movementSpeed = 10f;
    }

    // Get input
    private void Update()
    {
        _inputAxis = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(_hittingCharacter))
                _dialogueController.StartConversation(_hittingCharacter);
        }
    }

    // Move the player
    private void FixedUpdate()
    {
        if (!_canMove)
            return;

        _rb.velocity = new Vector2(_inputAxis * _movementSpeed, 0f);
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
}
