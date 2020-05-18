using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // Have to use inheritance instead of interfaces to avoid dependencies beacuse interfaces can't be serialized and assigned in the editor
    public abstract class BaseDialogueUIController : MonoBehaviour
    {
        public virtual IEnumerator ShowSentence(string speaker, TextModifications textMods, Sprite characterSprite, bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
        { return null; }

        public virtual void ShowOptions(List<Option> options)
        { }

        public virtual void OptionButtonClicked(int index)
        { }

        protected virtual void processCommand(string command)
        { }

        public virtual void Close()
        { }
    }
}
