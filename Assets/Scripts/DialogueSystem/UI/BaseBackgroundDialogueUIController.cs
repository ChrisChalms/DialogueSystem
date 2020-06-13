using System.Collections;
using UnityEngine;

namespace CC.DialogueSystem
{
    // The background conversation is a little different to the default conversation in that the UI controller is, for the most part, also the controller.
    public abstract class BaseBackgroundDialogueUIController : MonoBehaviour
    {
        protected Conversation _conversation;
        protected Dialogue _currentDialogue;

        protected int _currentSentence;
        protected float _speedMultiplyer;

        // Initialize
        public virtual void Initialize(Conversation conversation)
        {
            _conversation = conversation;
            findStartingPoint(conversation);

            if (_currentDialogue == null)
            {
                DialogueLogger.LogError($"Couldn't find starting point for background conversation");
                finishedConversation();
                return;
            }

            BackgroundDialogueController.Instance.CloseConversation += finishedConversation;

            startDialogue();
        }

        // Start a dialogue in the conversation
        protected virtual void startDialogue()
        {
            _currentSentence = -1; // Incremented in progress()
            progress();
        }

        // Progress in the conversation
        protected void progress()
        {
            if (_currentSentence >= _currentDialogue.Sentences.Count - 1)
            {
                // Progress to the next dialogue
                if (_currentDialogue.NextId != -1)
                {
                    var nextDialogue = _conversation.Dialogues.Find(d => d.Id == _currentDialogue.NextId);

                    if (nextDialogue == null)
                    {
                        DialogueLogger.LogError($"Trying to navigate to a dialogue with the Id {_currentDialogue.NextId}, but there's isn't one present in the current conversation");
                        return;
                    }
                    
                    _currentDialogue = nextDialogue;
                    startDialogue();
                }
                else
                    finishedConversation();
            }
            else
            {
                // There's more to show
                _currentSentence++;
                StartCoroutine(showSentence(parseSentenceForCustomTags(_currentDialogue.Sentences[_currentSentence])));
            }
        }

        // Show a sentence
        protected virtual IEnumerator showSentence(TextModifications textMod) => null;

        // Finished with the conversation
        protected virtual void finishedConversation() { }

        #region Tags and Actions

        // If you're UI supports tags, use this to execute all tags at a given position in the sentence
        // I don't like this here. But unfortunately, the default and background conversations are too different (in my example implementation) to put it in a parent class.
        // Please note, not all tags are used here
        protected IEnumerator processTagsForPosition(TextModifications textMod, int index)
        {
            var mods = textMod.GetAnyTextModsForPosition(index);

            // Check for custom modifications
            foreach (var mod in mods)
            {
                // Commands
                if (mod.ModType == TextModifications.Modifications.CLOSE_BG_CONVERSATIONS)
                    BackgroundDialogueController.Instance?.CloseConversations();

                // Simple modifications e.g. <command=value>
                if (mod.ModType == TextModifications.Modifications.SPEED)
                    _speedMultiplyer = (mod as SimpleModification).GetValue<float>();
                else if (mod.ModType == TextModifications.Modifications.REMOVE_VARAIBLE)
                    VariableRepo.Instance?.Remove((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.WAIT)
                    yield return new WaitForSeconds((mod as SimpleModification).GetValue<float>());
                else if (mod.ModType == TextModifications.Modifications.ACTION)
                    performAction((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG)
                    DialogueLogger.Log((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG_WARNING)
                    DialogueLogger.LogWarning((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG_ERROR)
                    DialogueLogger.LogError((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.BG_CONVERSATION)
                    BackgroundDialogueController.Instance?.StartConversation((mod as SimpleModification).GetValue<string>());

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
                else if (mod.ModType == TextModifications.Modifications.ACTION_WITH_MESSAGE)
                    performActionWithMessage((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>());
                else if (mod.ModType == TextModifications.Modifications.ACTION_WITH_TARGET)
                    performActionWithTarget((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>());
            }
        }

        // Pass actions to the action controller
        private void performAction(string actionName) => ActionController.PerformAction(_conversation, actionName);
        private void performActionWithMessage(string actionName, string message) => ActionController.PerformActionWithMessage(_conversation, actionName, message);
        private void performActionWithTarget(string actionName, string target) => ActionController.PerformActionWithTarget(_conversation, actionName, target);

        #endregion

        #region Helpers

        // Loop over the dialogues and find a starting point
        private void findStartingPoint(Conversation conversation)
        {
            // Find starting dialogue
            foreach (var diag in conversation.Dialogues)
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
        }

        // Returns the dialogue's anchor if found
        protected Transform getAnchor() => GameObject.Find(_currentDialogue.AnchorObject)?.transform;

        // Parse sentence and return TextModifications object
        private TextModifications parseSentenceForCustomTags(string sentence) => new TextModifications(sentence);

        #endregion
    }
}