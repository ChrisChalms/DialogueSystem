using UnityEngine;

namespace CC.DialogueSystem
{
    public class BaseDialogueController : MonoBehaviour
    {
        #region Helpers

        // Get the conversation from the repo if it exists
        protected Conversation findConversation(string convoName)
        {
            // Check if repo exists and the conversation has been loaded
            var tempoConvo = ConversationRepo.Instance?.RetrieveConversation(convoName);
            if (tempoConvo == null)
            {
                DialogueLogger.LogError($"Tried to start conversation {convoName} but it doesn't exist is the ConversationRepo");
                return null;
            }

            return tempoConvo;
        }

        #endregion
    }
}
