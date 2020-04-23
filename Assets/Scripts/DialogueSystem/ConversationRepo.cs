using System.Collections.Generic;
using UnityEngine;

// Parses and stores all the conversations for this scene. 
public class ConversationRepo : MonoBehaviour
{
    private static ConversationRepo _instance;

    [SerializeField]
    private List<TextAsset> _conversationsToLoad;

    private Dictionary<string, Conversation> _conversations;
    private IDeserializer _deserializer;

    public static ConversationRepo Instance => _instance;

    #region MonoBehaviour

    // Apply singleton
    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        new DialogueVariableRepo();
    }

    // Initialize
    private void Start()
    {
        _conversations = new Dictionary<string, Conversation>();
        _deserializer = new JSONDeserializer();

        loadConversations();
    }

    #endregion

    // Parse the text assets
    private void loadConversations()
    {
        foreach (var file in _conversationsToLoad)
        {
            var tempObject = _deserializer.Deserialize<Conversation>(file.text);
            if (valdateConversation(tempObject, file.name))
                _conversations[file.name] = tempObject; // Overwrite without throwing
        }
    }

    // Do simple validation on the conversation e.g. That it's not empy, that all nextIds go somewhere, things like that
    private bool valdateConversation(Conversation tempConversation, string fileName)
    {
        var dialogueIds = new List<int>();

        // Check number of dialogues
        if(tempConversation.Dialogues.Count == 0)
        {
            Debug.LogWarningFormat("Empty conversation in file {0}", fileName);
            return false;
        }

        // Check dialogues sentences and populate Id list
        foreach(var diag in tempConversation.Dialogues)
        {
            // Check sentence count
            if(diag.Sentences.Count == 0)
            {
                Debug.LogWarningFormat("Dialogue in file {0} has no sentences", fileName);
                return false;
            }

            // Check sentence for empty
            foreach (var sentence in diag.Sentences)
            {
                if (string.IsNullOrEmpty(sentence))
                {
                    Debug.LogWarningFormat("Empty sentence in file {0}", fileName);
                    return false;
                }
            }

            dialogueIds.Add(diag.Id);
        }

        // Loop through dialogues and check Ids and Options
        foreach(var diag in tempConversation.Dialogues)
        {
            // Check for dialogue next Ids values
            if(diag.NextId != -1)
            {
                if(!dialogueIds.Contains(diag.NextId))
                {
                    Debug.LogWarningFormat("Dialoge in file {0} references a dialogue by id {1} that doesn't exist", fileName, diag.NextId);
                    return false;
                }
            }

            // Check option texts and nextIds
            foreach(var option in diag.Options)
            {
                // I'm not sure about this. The idea was that if a dialogue has a next value, it can't have options. But what if you want to store/retrieve/trigger something on an option press?
                // TODO: Dialogue OnComplete actions might solve this issue. Just do it
                if(diag.NextId != -1)
                {
                    Debug.LogWarningFormat("Dialogue in file {0} has a nextId value, so can't have options", fileName);
                    return false;
                }

                // Check text
                if(string.IsNullOrEmpty(option.Text))
                {
                    Debug.LogWarningFormat("Empty option in file {0}", fileName);
                    return false;
                }

                // Check the next dialogue exists
                // TODO: Might need to change this if you want an option that goes nowhere, but performs an action if/when they ever get implemented
                if(!dialogueIds.Contains(option.NextId) || option.NextId == -1)
                {
                    Debug.LogWarningFormat("Dialogue option has a nextId {0} that doesn't exist, or it goes nowhere in file {1}", option.NextId, fileName);
                    return false;
                }
            }
        }

        return true;
    }

    // Resolve a registered conversation
    public Conversation ResolveConversation(string convoName) => _conversations.ContainsKey(convoName) ? _conversations[convoName] : null;
}
