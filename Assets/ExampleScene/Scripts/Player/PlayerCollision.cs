using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundLayers;
    [SerializeField]
    private Vector2 _groundCastOffset;
    [SerializeField]
    private float _groundCheckDistance;
    [SerializeField]
    private bool _drawCast;

    public bool IsGrounded { get; private set; }

    #region MonoBehaviour
    
    // Perform casts
    private void Update()
    {
        IsGrounded = Physics2D.Raycast((Vector2)transform.position + _groundCastOffset, Vector2.down, _groundCheckDistance, _groundLayers);
    }

    // Draw the ground check
    private void OnDrawGizmos()
    {
        if (!_drawCast)
            return;
        Gizmos.DrawLine((Vector2)transform.position + _groundCastOffset, new Vector2(transform.position.x + _groundCastOffset.x, transform.position.y + _groundCastOffset.y + _groundCheckDistance));
    }

    #endregion
}
