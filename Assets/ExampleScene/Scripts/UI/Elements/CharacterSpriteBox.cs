using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSpriteBox : MonoBehaviour
{
    private readonly float ON_X = 1645;
    private readonly float OFF_X = 1920;
    private readonly float TWEEN_TIME = 0.5f;
    private static readonly float ROTATION_MAX = 3;

    private RectTransform _thisRect;
    private Image _characterSprite;

    private bool _isShowing;

    #region MonoBehaviour

    // Initialize
    void Start()
    {
        _thisRect = transform as RectTransform;
        _characterSprite = transform.Find("CharacterSprite").GetComponent<Image>();

        _thisRect.anchoredPosition = new Vector2(OFF_X, _thisRect.anchoredPosition.y);
    }

    #endregion

    // Show the sprite and animate on if we need to
    public void ChangeSprite(Sprite newSprite)
    {
        if (newSprite == null)
        {
            hide();
            return;
        }

        _characterSprite.sprite = newSprite;

        if (!_isShowing)
            show();
    }

    // Shows the character sprite
    private void show()
    {
        _isShowing = true;
        _thisRect.DOAnchorPosX(ON_X, TWEEN_TIME);
        // Apply a random rotation to give it a little character
        _thisRect.rotation = Quaternion.Euler(0f, 0f, Random.Range(-ROTATION_MAX, ROTATION_MAX));
    }

    // Hide the character sprite
    private void hide()
    {
        // No need to animate
        if (!_isShowing)
        {
            _thisRect.anchoredPosition = new Vector2(OFF_X, _thisRect.anchoredPosition.y);
            return;
        }

        _isShowing = false;
        _thisRect.DOAnchorPosX(OFF_X, TWEEN_TIME);
    }
}