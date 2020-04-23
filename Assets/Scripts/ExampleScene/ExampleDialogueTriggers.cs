using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDialogueTriggers : MonoBehaviour
{
    private Vector2 GREEN_SHOW_POSITION = new Vector3(16.06f, 2.98f, 0);
    private float GREEN_TWEEN_TIME = 0.5f;

    [SerializeField]
    private SpriteRenderer _greenCharacter;
    [SerializeField]
    private Sprite _redSprite;

    private Transform _greenTransfrom;
    private Sprite _startingSprite;
    private Vector3 _startingPos;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _greenTransfrom = _greenCharacter.transform;
        _startingSprite = _greenCharacter.sprite;
        _startingPos = _greenTransfrom.position;
    }

    #endregion

    // Move the green guy's position to above the purple - We've got DOTween, might as well use it
    public void MoveGreenCharacter()
    {
        _greenTransfrom.DOMove(GREEN_SHOW_POSITION, GREEN_TWEEN_TIME);
    }

    // Move him back
    public void ResetPosition()
    {
        _greenTransfrom.DOMove(_startingPos, GREEN_TWEEN_TIME);
    }

    // Change the green character's sprite
    public void ChangeGreenCharacter()
    {
        _greenCharacter.sprite = _redSprite;
    }

    // Reset the green guy's sprite
    public void ResetGreenSprite()
    {
        _greenCharacter.sprite = _startingSprite;
    }
}
