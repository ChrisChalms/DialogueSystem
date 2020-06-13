using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRend;
    private PlayerCollision _collision;

    private int _yVelocityHash;
    private int _xVelocityHash;
    private int _isGroundedHash;

    #region MonoBehaviour

    // Initialize
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRend = GetComponent<SpriteRenderer>();
        _collision = GetComponentInParent<PlayerCollision>();

        // Get hashes
        _yVelocityHash = Animator.StringToHash("yVelocity");
        _xVelocityHash = Animator.StringToHash("xVelocityAbs");
        _isGroundedHash = Animator.StringToHash("isGrounded");
    }
    
    // Update animator
    private void Update() => _animator.SetBool(_isGroundedHash, _collision.IsGrounded);

    #endregion

    // Update the animator's velocity properties
    public void UpdateVelocity(Vector2 vels)
    {
        _animator.SetFloat(_xVelocityHash, Mathf.Abs(vels.x));
        _animator.SetFloat(_yVelocityHash, vels.y);
    }

    // Flip the sprite
    public void Flip(int facingDir) => _spriteRend.flipX = facingDir == -1 ? true : false;
}
