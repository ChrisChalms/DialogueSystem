using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Controls the little arrow that show the dialogue box is waiting for user input to proceed
public class NextArrow : MonoBehaviour
{
    private readonly float ALPHA_ANIMATION_TIME = 0.3f;
    private readonly int BOUNCE_AMOUNT = 20;
    private readonly float BOUNCE_TIME = 0.5f;

    private Image _thisImage;
    private RectTransform _thisRect;

    private bool _showing;
    private Vector2 _startingPos;
    private bool _bouncingDown;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _thisImage = GetComponent<Image>();
        _thisRect = transform as RectTransform;
        _startingPos = _thisRect.anchoredPosition;
    }

    #endregion

    // Show the arrow and start the bounce
    public void Show()
    {
        _showing = true;
        _thisRect.anchoredPosition = _startingPos;
        _thisImage.DOFade(1f, ALPHA_ANIMATION_TIME);
        _bouncingDown = false; // Gives the delay of one bounce before starting
        bounce();
    }

    // Hide the arrow and stop bounce on the next cycle
    public void Hide()
    {
        _showing = false;
        _thisImage.DOFade(0f, ALPHA_ANIMATION_TIME);
    }

    // Animates the arrow up or down
    private void bounce()
    {
        if (!_showing)
            return;

        _bouncingDown = !_bouncingDown;

        _thisRect.DOAnchorPosY(_bouncingDown ? _startingPos.y : _startingPos.y + BOUNCE_AMOUNT, BOUNCE_TIME).SetEase(Ease.InOutQuad).OnComplete(bounce);
    }
}