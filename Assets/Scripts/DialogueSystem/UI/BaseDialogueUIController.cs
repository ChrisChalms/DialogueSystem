using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // A base for the UI controller that handles the seup for a sentence and tags, actual UI implementation by children
    public abstract class BaseDialogueUIController : MonoBehaviour
    {
        protected float _speedMultiplyer;
        protected TextModifications _currentTextMod;

        // Pre ShowSentence()
        public void PrepareToShowSentence(string speaker, TextModifications textMods, Sprite characterSprite, bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
        {
            _speedMultiplyer = 1;
            _currentTextMod = textMods;
            StartCoroutine(ShowSentence(speaker, characterSprite, sameSpeakerAsLastDialogue, autoProceed));
        }

        // Shows the sentence, implement in children
        public virtual IEnumerator ShowSentence(string speaker, Sprite characterSprite, bool sameSpeakerAsLastDialogue = true, bool autoProceed = false) => null;

        // If you're UI supports tags, use this to execute all tags at a given position in the sentence
        protected IEnumerator processTagsForPosition(int index)
        {
            var mods = _currentTextMod?.GetAnyTextModsForPosition(index);

            // Check for custom modifications
            foreach (var mod in mods)
            {
                // Commands
                if (mod.ModType == TextModifications.Modifications.HIDE_SPRITE)
                    hideCharacterSprite();

                // Simple modifications e.g. <command=value>
                else if (mod.ModType == TextModifications.Modifications.SPEED)
                    _speedMultiplyer = (mod as SimpleModification).GetValue<float>();
                else if (mod.ModType == TextModifications.Modifications.REMOVE_VARAIBLE)
                    VariableRepo.Instance.Remove((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.WAIT)
                    yield return new WaitForSeconds((mod as SimpleModification).GetValue<float>());
                else if (mod.ModType == TextModifications.Modifications.ACTION)
                    DialogueController.Instance.PerformAction((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG)
                    DialogueLogger.Log((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG_WARNING)
                    DialogueLogger.LogWarning((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.LOG_ERROR)
                    DialogueLogger.LogError((mod as SimpleModification).GetValue<string>());
                else if (mod.ModType == TextModifications.Modifications.CHANGE_THEME)
                    DialogueController.Instance.ChangeTheme((mod as SimpleModification).GetValue<string>());

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
                    changeCharacterSprite(SpriteRepo.Instance.RetrieveSprite((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>()));
                else if (mod.ModType == TextModifications.Modifications.ACTION_WITH_MESSAGE)
                    DialogueController.Instance.PerformActionWithMessage((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>());
                else if (mod.ModType == TextModifications.Modifications.ACTION_WITH_TARGET)
                    DialogueController.Instance.PerformActionWithTarget((mod as SimpleModification).GetValue<string>(), (mod as ComplexModification).GetContent<string>());
            }
        }

        // Implement the below if you UI has options
        public virtual void ShowOptions(List<Option> options) { }

        public virtual void OptionButtonClicked(int index) { }

        // Implement the below if your UI has character sprites
        protected virtual void changeCharacterSprite(Sprite newSprite) { }

        protected virtual void hideCharacterSprite() { }

        // Hide all your UI elements
        public virtual void Close() { }
    }
}
