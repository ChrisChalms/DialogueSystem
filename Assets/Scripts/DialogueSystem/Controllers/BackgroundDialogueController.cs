#pragma warning disable 649

using System;
using UnityEngine;

namespace CC.DialogueSystem
{
    // Starting point for finding and starting background conversations. This is a lot simpler than the default DialogueController, we just find the conversation, spawn a new BaseBackgroundDialogueUIController, and give it the conversation
    public class BackgroundDialogueController : BaseDialogueController
    {
        // Actions
        public event Action CloseConversation;

        [SerializeField]
        private BaseBackgroundDialogueUIController _uiControllerPrefab;

        public static BackgroundDialogueController Instance { get; private set; }

        #region MonoBehaviour

        // Apply singleton
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        #endregion

        // Find in the repo and start
        public override void StartConversation(string convoName)
        {
            var tempConvo = findConversation(convoName);

            if (tempConvo == null)
                return;

            // Check if it's a normal conversation and redirect
            if (tempConvo.ConversationType == Conversation.Types.DEFAULT)
            {
                DialogueLogger.Log($"Trying to start conversation {convoName} as a background conversation, redirecting to the default DialogueController");
                DialogueController.Instance?.StartConversation(tempConvo);
                return;
            }

            StartConversation(tempConvo);
        }

        // Already found, just start
        public override void StartConversation(Conversation conversation)
        {
            if (_uiControllerPrefab == null)
            {
                DialogueLogger.LogError("Trying to start a background conversation, but there's not BaseBackgroundDialogueUIController assigned");
                return;
            }

            var tempObject = Instantiate(_uiControllerPrefab).GetComponent<BaseBackgroundDialogueUIController>();
            
            if(tempObject == null)
            {
                DialogueLogger.LogError($"Background conversation spawned does't have a component of type BaseBackgroundDialogueUIController");
                return;
            }

            tempObject.Initialize(conversation);
        }

        // Close all background conversations
        public void CloseConversations() => CloseConversation?.Invoke();
    }
}