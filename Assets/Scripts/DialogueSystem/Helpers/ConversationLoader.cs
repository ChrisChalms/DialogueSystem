#pragma warning disable 649

using UnityEngine;

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
            LoadConversation();
    }

    #endregion

    // Sends the conversation to the repo
    public void LoadConversation()
    {
        // Can't load if there's no name
        if (string.IsNullOrEmpty(_name))
        {
            Debug.LogWarningFormat("Object {0} cannot register a conversation without a name", gameObject.name);
            return;
        }

        // Can't load if there's no conversation
        if(_file == null)
        {
            Debug.LogWarningFormat("Object {0} has an empty conversation file", gameObject.name);
            return;
        }

        ConversationRepo.Instance.RegisterConversation(_name, _file);
    }
}
