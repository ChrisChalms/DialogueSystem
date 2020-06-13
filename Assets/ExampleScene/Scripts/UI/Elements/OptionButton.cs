using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CC.DialogueSystem;

// A single option button, simply controls its visual state and tell the UI when it's been selected
public class OptionButton : MonoBehaviour
{
    private static readonly float ON_Y          = -100;
    private static readonly float OFF_Y         = -250;
    private static readonly float TWEEN_TIME    = 0.5f;
    private static readonly float ROTATION_MAX  = 3; 
    
    private RectTransform _thisRect;
    private RectTransform _guideRect;
    private TextMeshProUGUI _text;
    private static BaseDialogueUIController _uiController;

    private int _buttonNumber;

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        _thisRect = transform as RectTransform;
        _guideRect = transform.parent.Find("Guide" + gameObject.name) as RectTransform;
        _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        if (_uiController == null)
            _uiController = GameObject.Find("DialogueCanvas/DialogueUI").GetComponent<BaseDialogueUIController>();

        _buttonNumber = int.Parse(gameObject.name);

        _thisRect.anchoredPosition = new Vector2(_thisRect.anchoredPosition.x, OFF_Y);
    }

    #endregion

    // Change the button's text
    public IEnumerator SetText(string optionText)
    {
        _text.text = optionText;

        yield return null; // Wait for contentSizeFitter to update

        // Set guide object's width
        _guideRect.sizeDelta = _thisRect.sizeDelta;
    }

    // Animate the button up
    public void Show()
    {
        _thisRect.anchoredPosition = new Vector2(_guideRect.anchoredPosition.x, OFF_Y);

        // Apply a random rotation to give it a little character
        _thisRect.rotation = Quaternion.Euler(0f, 0f, Random.Range(-ROTATION_MAX, ROTATION_MAX));
        _thisRect.DOAnchorPos(new Vector2(_guideRect.anchoredPosition.x, ON_Y), TWEEN_TIME).SetEase(Ease.OutQuad);
    }

    // Animate the button down
    public void Hide() => _thisRect.DOAnchorPos(new Vector2(_guideRect.anchoredPosition.x, OFF_Y), TWEEN_TIME).SetEase(Ease.InQuad);

    // The option's been selected, tell the UI
    public void OptionSelected() => _uiController.OptionButtonClicked(_buttonNumber);

    // Enable this button's guide object
    public void Enable()
    {
        _guideRect.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    // Disable this button's guide object
    public void Disable()
    {
        _guideRect.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
