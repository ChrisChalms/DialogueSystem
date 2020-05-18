﻿#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CC.DialogueSystem;

// Example of how the UI would work with the DialogueController to show text, mostly just passes the calls to the separate elements of the UI and handles the custom tags
public class ExampleDialogueUIController : BaseDialogueUIController
{
    [SerializeField]
    private float _defaultTimeBetweenChars = 0.035f;
    [SerializeField]
    private float _fastTimebetweenChars = 0.01f;

    private DialogueBox _dialogueBox;
    private NameBox _nameBox;
    private CharacterSpriteBox _spriteBox;
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
        _spriteBox = transform.Find("CharacterSpriteMask").GetComponent<CharacterSpriteBox>();
        _optionButtons = transform.Find("Options").GetComponent<OptionsButtonController>();
        _nextArrow = transform.Find("NextArrow").GetComponent<NextArrow>();
        _firstSentenceDelay = new WaitForSeconds(0.3f);

        _currentTimeBetweenChars = _defaultTimeBetweenChars;

        // Actions example
        DialogueVariableRepo.Instance.VariableRegistered += (key) => DialogueLogger.Log($"The variable {key} was added");
        DialogueVariableRepo.Instance.VariableUpdated += (key) => DialogueLogger.Log($"The variable {key} was updated");
        DialogueVariableRepo.Instance.VariableRemoved += (key) => DialogueLogger.Log($"The variable {key} was removed");
    }

    // Handle the UI Input
    private void Update()
    {
        // If the user is pressing the mouse down speed up the text if we're animating, otherwise proceed in the conversation
        if (Input.GetMouseButton(0))
        {
            // Speed up text if we're going the default speed
            if (_isAnimatingText)
            {
                _currentTimeBetweenChars = (_speedMultiplyer == 1) ? _fastTimebetweenChars : _defaultTimeBetweenChars;

                _handledInput = true;
            }
            // Proceed if we're not animating
            else
            {
                if (!_handledInput)
                {
                    if (!_optionButtons.IsShowingOptions)
                    {
                        DialogueController.Instance.Next();
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
    public override IEnumerator ShowSentence(string speakersName, TextModifications textMods, Sprite characterSprite, bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
    {
        _isAnimatingText = true;
        _speedMultiplyer = 1;
        _nextArrow.Hide();
        _dialogueBox.Show(false);
        _dialogueBox.SetSentence(textMods.Sentence);
        _nameBox.SetName(speakersName);
        _optionButtons.HideOptions();
        if (!sameSpeakerAsLastDialogue)
            _spriteBox.ChangeSprite(characterSprite);

        // This is the first sentence, wait a little for the box to fully animate into place
        if (!_isShowing)
        {
            _isShowing = true;
            _spriteBox.ChangeSprite(characterSprite);
            yield return _firstSentenceDelay;
        }

        // Check for and apply any mods, then progress through the sentence
        for (var i = 0; i < textMods.Sentence.Length; i++)
        {
            // Check for custom modifications
            var mods = textMods.GetAnyTextModsForPosition(i);
            foreach (var mod in mods)
            {
                // Commands
                if (mod.ModType == TextModifications.Modifications.HIDE_SPRITE)
                    _spriteBox.ChangeSprite(null);

                // Simple modifications e.g. <command=value>
                else if (mod.ModType == TextModifications.Modifications.SPEED)
                    _speedMultiplyer = (mod as SimpleModification).GetValue<float>();
                else if (mod.ModType == TextModifications.Modifications.REMOVE_VARAIBLE)
                    DialogueVariableRepo.Instance.Remove((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.WAIT)
                    yield return new WaitForSeconds((mod as SimpleModification).GetValue<float>());
                else if (mod.ModType == TextModifications.Modifications.PERFORM_ACTION)
                    DialogueController.Instance.PerformAction((mod as SimpleModification).GetValue<string>());

                // Complex modifications e.g. <command=value>content</command>
                else if (mod.ModType == TextModifications.Modifications.SEND_MESSAGE)
                {
                    var revievingObject = GameObject.Find((mod as SimpleModification).GetValue<string>());
                    if (revievingObject == null)
                    {
                        DialogueLogger.LogError($"Trying to execute a send message command, but GameObject {(mod as SimpleModification).GetValue<string>()} was not found");
                        continue;
                    }

                    revievingObject.SendMessage((mod as ComplexModification).GetContent<string>(), SendMessageOptions.DontRequireReceiver);
                }
                else if (mod.ModType == TextModifications.Modifications.CHANGE_SPRITE)
                    _spriteBox.ChangeSprite(DialogueSpriteRepo.Instance.RetrieveSprites((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>()));
            }

            _dialogueBox.IncrementVisibleCharacters();
            yield return new WaitForSeconds(_currentTimeBetweenChars * _speedMultiplyer);
        }

        _isAnimatingText = false;

        if (autoProceed)
            DialogueController.Instance.Next();
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
        DialogueController.Instance.OptionSelected(index);
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