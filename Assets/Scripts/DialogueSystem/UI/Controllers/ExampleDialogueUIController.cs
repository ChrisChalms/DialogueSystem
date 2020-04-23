using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of how the UI would work with the DialogueController to show text, mostly just passes the calls to the separate elements of the UI and handles the custom tags
public class ExampleDialogueUIController : BaseDialogueUIController
{
    [SerializeField]
    private float _defaultTimeBetweenChars = 0.08f;
    [SerializeField]
    private float _fastTimebetweenChars = 0.01f;

    private DialogueBox _dialogueBox;
    private NameBox _nameBox;
    private OptionsButtonController _optionButtons;
    private NextArrow _nextArrow;
    private WaitForSeconds _firstSentenceDelay; // Time to wait before starting to write the text if the box isn't already in place

    private bool _isShowing;
    private bool _isAnimatingText;
    private float _currentTimeBetweenChars;
    private float _speedMultiplyer;
    private bool _handledInput;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _dialogueBox = GetComponent<DialogueBox>();
        _nameBox = transform.Find("NameBox").GetComponent<NameBox>();
        _optionButtons = transform.Find("Options").GetComponent<OptionsButtonController>();
        _nextArrow = transform.Find("NextArrow").GetComponent<NextArrow>();
        _firstSentenceDelay = new WaitForSeconds(0.3f);

        _currentTimeBetweenChars = _defaultTimeBetweenChars;
    }

    // Handle the UI Input
    private void Update()
    {
        // If the user is pressing the mouse down speed up the text if we're animating, otherwise proceed in the conversation
        if(Input.GetMouseButton(0))
        {
            // Speed up text if we're going the default speed
            if(_isAnimatingText)
            {
                _currentTimeBetweenChars = (_speedMultiplyer == 1) ? _fastTimebetweenChars : _defaultTimeBetweenChars;

                _handledInput = true;
            }
            // Proceed if we're not animating
            else
            {
                if(!_handledInput)
                {
                    if (!_optionButtons.IsShowingOptions)
                    {
                        DialogueController.Next();
                        _handledInput = true;
                    }
                }
            }
        }
        else
        {
            _currentTimeBetweenChars = _defaultTimeBetweenChars;
            _handledInput = false;
        }
    }

    #endregion

    // Animate the sentence text and handle the custom tags
    public override IEnumerator ShowSentence(string speaker, TextModifications textMods, bool autoProceed = false)
    {
        _isAnimatingText = true;
        _speedMultiplyer = 1;
        _nextArrow.Hide();
        _dialogueBox.Show(false);
        _dialogueBox.SetSentence(textMods.Sentence);
        _nameBox.SetName(speaker);
        _optionButtons.HideOptions();

        // This is the first sentence, wait a little for the box to fully animate into place
        if (!_isShowing)
        {
            _isShowing = true;
            yield return _firstSentenceDelay;
        }

        // Check for and apply any mods, then progress through the sentence
        for(var i = 0; i < textMods.Sentence.Length; i++)
        {
            // Check for custom modifications
            var mods = textMods.GetAnyTextModsForPosition(i);
            foreach (var mod in mods)
            {
                if (mod.ModType == TextModifications.Modifications.SPEED)
                    _speedMultiplyer = mod.GetValue <float>();
                else if(mod.ModType == TextModifications.Modifications.SEND_MESSAGE)
                {
                    if (DialogueMessageTarget == null)
                        SendMessage(mod.GetValue<string>(), SendMessageOptions.DontRequireReceiver);
                    else
                        DialogueMessageTarget.SendMessage(mod.GetValue<string>(), SendMessageOptions.DontRequireReceiver);
                }
            }

            _dialogueBox.IncrementVisibleCharacters();
            yield return new WaitForSeconds(_currentTimeBetweenChars * _speedMultiplyer);
        }

        _isAnimatingText = false;

        if (autoProceed)
            DialogueController.Next();
        else
            _nextArrow.Show();
    }

    // Show the options
    public override void ShowOptions(List<Option> options)
    {
        _dialogueBox.Show(true);
        StartCoroutine(_optionButtons.ShowOptions(options));
    }

    // An option was selected
    public override void OptionButtonClicked(int index)
    {
        DialogueController.OptionSelected(index);
    }

    // Close the UI
    public override void Close()
    {
        _isShowing = false;
        _nextArrow.Hide();
        _dialogueBox.Hide();
        _nameBox.SetName(string.Empty);
        _optionButtons.HideOptions();
    }
}
