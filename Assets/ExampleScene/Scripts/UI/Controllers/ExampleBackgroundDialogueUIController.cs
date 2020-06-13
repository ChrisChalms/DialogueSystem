#pragma warning disable 649

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CC.DialogueSystem;

// Example of a background conversation. Gets handed a conversation from the BackgroundDialogueController and controls everything here
public class ExampleBackgroundDialogueUIController : BaseBackgroundDialogueUIController
{
    private static readonly float FADE_TIME                     = 0.3f;
    private static readonly float FAST_FADE_TIME                = 0.1f;
    private static readonly WaitForSeconds STARTING_DELAY       = new WaitForSeconds(0.4f);
    private static readonly WaitForSeconds ANCHOR_CHANGE_DELAY  = new WaitForSeconds(0.4f);
    private static readonly WaitForSeconds END_DELAY            = new WaitForSeconds(1f);

    [SerializeField]
    private Vector2 _anchorOffset; // Only going to have one for the example
    [SerializeField]
    private float _defaultTimeBetweenChars = 0.1f;

    private RectTransform _thisRect;
    private Image _dialogueBackground;
    private TextMeshProUGUI _text;
    private Canvas _canvas;
    private Transform _lastAnchor;

    private float _currentTimeBetweenChars;
    private bool _anchorChanged;
    private bool _isStartingtSentence;

    #region BaseBackgroundDialogueUIController

    // Initialize
    public override void Initialize(Conversation conversation)
    {
        _dialogueBackground = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _canvas = GameObject.Find("DialogueCanvas").GetComponent<Canvas>();

        _currentTimeBetweenChars = _defaultTimeBetweenChars;
        _isStartingtSentence = true;

        changeParent();

        base.Initialize(conversation);
    }

    // Find the anchor if it's present and fade in background if needed
    protected override void startDialogue()
    {
        // Move the box to the anchor position
        var anchor = getAnchor();
        if(anchor != null && anchor != _lastAnchor)
        {
            // Move to position + offset. Animate if we're showing, otherwise snap
            var targetPosition = worldToUISpace(_canvas, anchor.position) + _anchorOffset;
            if (_isStartingtSentence)
                _thisRect.anchoredPosition = targetPosition;
            else
                StartCoroutine(fadeAndMove(targetPosition));
            _lastAnchor = anchor;
            _anchorChanged = true;
        }

        base.startDialogue();
    }

    // Animate the text and handle any custom tags
    protected override IEnumerator showSentence(TextModifications textMod)
    {
        _speedMultiplyer = 1;

        // If the anchor has just changed, wait a little longer before setting the text (and resizing the box)
        if (_anchorChanged && !_isStartingtSentence)
        {
            _anchorChanged = false;
            yield return ANCHOR_CHANGE_DELAY;
            _text.text = textMod.Sentence;
        }
        else
            _text.text = textMod.Sentence;

        _text.maxVisibleCharacters = 0;
        fadeBackground(1);
        _isStartingtSentence = false;

        // Delay a little before displaying text
        yield return STARTING_DELAY;

        // Check for and apply and mods, then progress through the sentence
        for (var i = 0; i < textMod.Sentence.Length; i++)
        {
            yield return StartCoroutine(processTagsForPosition(textMod, i));
            _text.maxVisibleCharacters++;
            yield return new WaitForSeconds(_currentTimeBetweenChars * _speedMultiplyer);
        }

        yield return END_DELAY;

        progress();
    }

    // Finished with the conversation, fade out and destroy
    protected override void finishedConversation()
    {
        fadeBackground(0);
        Destroy(gameObject, FADE_TIME * 1.1f);
    }

    #endregion

    #region Helpers

    // Change the parent if it can be found
    private void changeParent()
    {
        _thisRect = transform as RectTransform;
        var parentObject = GameObject.Find("DialogueCanvas/BackgroundConversations")?.transform;
        if (parentObject != null)
            _thisRect.SetParent(parentObject);
    }

    // Fade the background to the desired alpha
    private void fadeBackground(float newFade, bool fast = false)
    {
        _dialogueBackground.DOFade(newFade, fast ? FAST_FADE_TIME : FADE_TIME);
        _text.DOFade(newFade, fast ? FAST_FADE_TIME : FADE_TIME);
    }

    // Fade out then move
    private IEnumerator fadeAndMove(Vector2 position)
    {
        fadeBackground(0);
        yield return new WaitForSeconds(FADE_TIME);
        _thisRect.anchoredPosition = position;
    }

    // Convert the world position to UI
    public Vector2 worldToUISpace(Canvas parentCanvas, Vector3 anchorPos)
    {
        var screenPos = Camera.main.WorldToScreenPoint(anchorPos);

        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);

        return movePos;
    }

    #endregion
}
