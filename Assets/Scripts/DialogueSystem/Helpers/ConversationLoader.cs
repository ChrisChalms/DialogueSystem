#pragma warning disable 649

using UnityEngine;

namespace CC.DialogueSystem
{
    public class ConversationLoader : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _file;
        [SerializeField]
        private string _name;
        [SerializeField]
        private bool _registerOnStart;

        #region MonoBehaviour

        // Initialize
        private void Start()
        {
            if (_registerOnStart)
                RegisterConversation();
        }

        #endregion

        public void RegisterConversation() => ConversationRepo.Instance.RegisterConversation(_name, _file);
    }
}