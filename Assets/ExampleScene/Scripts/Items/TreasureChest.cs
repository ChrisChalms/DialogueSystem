#pragma warning disable 649

using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private Animator _animator;

    private bool _isOpen;

    #region MonoBehaviour

    // Initialize
    private void Start() => _animator = GetComponent<Animator>();

    #endregion

    // Plays the chest opening animation if the chest is closed
    public void Open()
    {
        if (_isOpen)
            return;

        _animator.SetTrigger("open");
    }

}
