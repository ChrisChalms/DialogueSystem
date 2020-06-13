using UnityEngine;
using DG.Tweening;
using TMPro;

// Controls the main dialogue box and the sentence text 
public class DialogueBox : MonoBehaviour
{
    private readonly float ON_Y = 20;
    private readonly float OFF_Y = -640;
    private readonly float SHOWING_OPTIONS_Y = 200;
    private readonly float TWEEN_TIME = 0.3f;

    private RectTransform _thisRect;
    private TextMeshProUGUI _dialogueText;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _thisRect = transform as RectTransform;
        _dialogueText = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        _thisRect.anchoredPosition = new Vector2(_thisRect.anchoredPosition.x, OFF_Y);
    }

    #endregion

    // Show the dialogue, with or without options
    public void Show(bool options) => _thisRect.DOAnchorPosY(options ? SHOWING_OPTIONS_Y : ON_Y, TWEEN_TIME);

    // Hide the dialogue box
    public void Hide() => _thisRect.DOAnchorPosY(OFF_Y, TWEEN_TIME);

    // Sets the text ready for animating
    public void SetSentence(string sentence)
    {
        _dialogueText.text = sentence;
        _dialogueText.maxVisibleCharacters = 0;
    }

    // Shows more of the text
    public void IncrementVisibleCharacters() => _dialogueText.maxVisibleCharacters++;
}
