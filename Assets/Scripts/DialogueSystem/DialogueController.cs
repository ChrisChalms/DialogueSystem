#pragma warning disable 649

using System;
using UnityEngine;

namespace CC.DialogueSystem
{
    // The starting point for starting conversations and proceeding in the current conversation
    public class DialogueController : MonoBehaviour
    {
        // Actions
        public event Action ConversationStarted;
        public event Action ConversationEnded;

        [SerializeField]
        private DialogueLogger.LogLevel _logLevel;
        [SerializeField]
        private BaseDialogueUIController _uiController;

        private Conversation _currentConversation;
        private Dialogue _currentDialogue;

        private bool _inConversation;
        private int _currentSentence;
        private string _lastSpeaker;

        public static DialogueController Instance { get; private set; }

        #region MonoBehaviour

        // Apply singleton
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            DialogueLogger.CurrentLogLevel = (int)_logLevel;
        }

        #endregion

        // Start the conversation if possible
        public void StartConversation(string convoName)
        {
            if (_inConversation)
            {
                DialogueLogger.LogWarning($"Trying to start conversation {convoName} while already in a conversation");
                return;
            }

            if (_uiController == null)
            {
                DialogueLogger.LogError("Trying to start a conversation, but there's not DialogueUIController assigned");
                return;
            }

            // Check if repo exists and the conversation has been loaded
            var tempoConvo = DialogueConversationRepo.Instance?.RetrieveConversation(convoName);
            if (tempoConvo == null)
            {
                DialogueLogger.LogError($"Tried to start conversation {convoName} but it doesn't exist is the ConversationRepo");
                return;
            }

            // Find starting point
            _currentConversation = null;
            _currentDialogue = null;
            _currentSentence = 0;
            foreach (var diag in tempoConvo.Dialogues)
            {
                if ((_currentDialogue == null || diag.Id > _currentDialogue.Id) && diag.CanBeUsedAsStartingPoint)
                {
                    if (diag.StartConditions.Count > 0)
                    {
                        if (diag.EvaluateStartingConditions())
                            _currentDialogue = diag;
                    }
                    else
                        _currentDialogue = diag;
                }
            }

            if (_currentDialogue == null)
            {
                DialogueLogger.LogError($"Couldn't find starting point for conversation {convoName}");
                return;
            }

            // Start the conversation
            _inConversation = true;
            _currentConversation = tempoConvo;

            StartCoroutine(_uiController.ShowSentence(_currentDialogue.SpeakersName,
                parseSentenceForCustomTags(_currentDialogue.Sentences[_currentSentence]),
                DialogueSpriteRepo.Instance.RetrieveSprites(_currentDialogue.CharacterSpritesName, string.IsNullOrEmpty(_currentDialogue.StartingSprite) ? "Default" : _currentDialogue.StartingSprite),
                _currentDialogue.AutoProceed));
            ConversationStarted?.Invoke();
        }

        // Go to the next step in the conversation e.g. Show options, go to the next dialogue in conversation, or go to the next sentence
        public void Next()
        {
            if (!_inConversation)
                return;

            if (_currentSentence >= _currentDialogue.Sentences.Count - 1)
            {
                // Check if there's options to display
                if (_currentDialogue.Options.Count != 0)
                    _uiController.ShowOptions(_currentDialogue.Options);
                // Check if we need to go to another dialogue
                else if (_currentDialogue.NextId != -1)
                {
                    goToDialogue(_currentDialogue.NextId);
                }
                // We've finished. Might need to check about actions here when we've finished the conversation
                else
                    StopCurrentConversation();
            }
            else
            {
                // There's more sentences to show
                _currentSentence++;

                var sameSpeaker = _lastSpeaker == _currentDialogue.SpeakersName;
                _lastSpeaker = _currentDialogue.SpeakersName;

                StartCoroutine(_uiController.ShowSentence(_currentDialogue.SpeakersName,
                    parseSentenceForCustomTags(_currentDialogue.Sentences[_currentSentence]),
                    (!sameSpeaker) ? DialogueSpriteRepo.Instance.RetrieveSprites(_currentDialogue.CharacterSpritesName, string.IsNullOrEmpty(_currentDialogue.StartingSprite) ? "Default" : _currentDialogue.StartingSprite) : null, //null, // After the conversation has started the sprite should be changed using the changeSprite tag
                    sameSpeaker,
                    _currentDialogue.AutoProceed));
            }
        }

        // An option was selected
        public void OptionSelected(int index)
        {
            // Perform any selected action
            foreach (var actionName in _currentDialogue.Options[index].SelectedActionNames)
            {
                // Get the action
                var action = _currentConversation.Actions.Find(a => a.Name == actionName);

                if (action != null)
                    performAction(action);
                else
                    DialogueLogger.LogError("Cannot find the selectedAction for the option selected. Skipping action");
            }

            if (_currentDialogue.Options[index].NextId != -1)
                goToDialogue(_currentDialogue.Options[index].NextId);
        }

        #region Helpers

        // Called from the UIController, used by the performAction custom tag
        public void PerformAction(string actionName)
        {
            // Get the action
            var action = _currentConversation.Actions.Find(a => a.Name == actionName);

            if (action != null)
                performAction(action);
            else
                DialogueLogger.LogError("Cannot find the selectedAction for the option selected. Skipping action");
        }

        // Performs the supplied action
        private void performAction(DialogueAction action)
        {
            switch (action.ActionType)
            {
                case DialogueAction.Types.LOG: DialogueLogger.Log(action.Message); break;
                case DialogueAction.Types.LOG_WARNING: DialogueLogger.LogWarning(action.Message); break;
                case DialogueAction.Types.LOG_ERROR: DialogueLogger.LogError(action.Message); break;

                case DialogueAction.Types.CLOSE_CONVERSATION: StopCurrentConversation(); break;

                case DialogueAction.Types.SEND_MESSAGE:
                    var targetObject = GameObject.Find(action.Target);

                    if (targetObject == null)
                    {
                        DialogueLogger.LogError($"Trying to execute a send message action, but GameObject {action.Target} was not found. Skipping action");
                        return;
                    }
                    targetObject.SendMessage(action.Message, SendMessageOptions.DontRequireReceiver);
                    break;
                default:
                    DialogueLogger.LogError($"Action with the name {action.Name} has na unrecognised action type {action.ActionType}. The action type loaded from the conversation JSON is {action.Type}. Skipping action");
                    break;
            }
        }

        // Navigates to a dialogue inside the current conversation
        private void goToDialogue(int index)
        {
            var nextDialogue = _currentConversation.Dialogues.Find(d => d.Id == index);

            if (nextDialogue == null)
            {
                DialogueLogger.LogError($"Trying to navigate to a dialogue with the Id {index}, but there's isn't one present in the current conversation");
                return;
            }

            _lastSpeaker = _currentDialogue.SpeakersName;
            _currentDialogue = nextDialogue;
            _currentSentence = -1; // Incremented in Next()
            Next();
        }

        // Closes the conversation, can be used manually or as a dialogue action
        public void StopCurrentConversation()
        {
            _uiController.Close();
            _inConversation = false;
            ConversationEnded?.Invoke();
        }

        // Parse sentence and return TextModifications object
        private TextModifications parseSentenceForCustomTags(string sentence) => new TextModifications(sentence);

        #endregion
    }
}
