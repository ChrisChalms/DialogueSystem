#pragma warning disable 649

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

        // Initialize early so it's ready for use in Start()
        _conversations = new Dictionary<string, Conversation>();
        _deserializer = new JSONDeserializer();

        loadConversations();
    }

    #endregion

    // Parse the text assets that have been assigned in the editor
    private void loadConversations()
    {
        foreach (var file in _conversationsToLoad)
        {
            var tempObject = _deserializer.Deserialize<Conversation>(file.text);
            if (tempObject == null)
            {
                Debug.LogWarningFormat("There was an error deserializing file {0}", file.name);
                return;
            }

            if (valdateConversation(tempObject, file.name))
            {
                _conversations[file.name] = tempObject; // Overwrite without throwing
                tempObject.FinishedParsing();
            }
        }
    }

    // Register and validate a conversation in real-time
    public void RegisterConversation(string name, TextAsset file)
    {
        var tempObject = _deserializer.Deserialize<Conversation>(file.text);
        if(tempObject == null)
        {
            Debug.LogWarningFormat("There was an error deserializing file {0}", file.name);
            return;
        }

        if (valdateConversation(tempObject, file.name))
        {
            if (_conversations.ContainsKey(name))
                Debug.LogWarningFormat("Conversation {0} already registered, overwritting", name);

            _conversations[name] = tempObject;
            tempObject.FinishedParsing();
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

        // Loop through dialogues and check Ids, Conditions, and Options
        var startingPriorities = new List<int>();
        foreach (var diag in tempConversation.Dialogues)
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

            // Check Conditions and Variables
            foreach (var con in diag.StartConditions)
            {
                // Can only test 2 varaibles against each other. When a conversation is picked all conditions must evaluate to true, so if more tests are needed just stack conditions
                if (con.Variables.Count != 2)
                {
                    Debug.LogWarningFormat("A condition in file {0} does not contains {1} variables, must have 2", fileName, con.Variables.Count);
                    return false;
                }

                // Check the type, value, and name individually
                foreach(var variable in con.Variables)
                {
                    // Check there's a value if we're not retrieving from the repo
                    if (string.IsNullOrEmpty(variable.Value))
                    {
                        if (!variable.FromRepo)
                        {
                            Debug.LogWarningFormat("A condition variable in file {0} has a conditional varialbe without a value. One is required if not retrieving from the variable repo", fileName);
                            return false;
                        }
                    }
                    else
                    {
                        if(variable.FromRepo)
                            Debug.LogWarningFormat("A condition variable in file {0} has a value it will be ignored as the variable is marked to be retrieved from the variable repo", fileName);
                    }

                    // A type is required if we're not retrieving from the repo
                    if (string.IsNullOrEmpty(variable.Type))
                    {
                        if (!variable.FromRepo)
                        {
                            Debug.LogWarningFormat("A condition in file {0} has a conditional variable without a type. One is required if not retrieving from the variable repo", fileName);
                            return false;
                        }
                    }
                    else
                    {
                        if(variable.FromRepo)
                            Debug.LogWarningFormat("A condition variable in file {0} has a type but will be ignored as the variable is marked to be retrieved from the variable repo", fileName);
                    }

                    // Check there's a name if we're retrieving from the repo
                    if(string.IsNullOrEmpty(variable.Name))
                    {
                        if(variable.FromRepo)
                        {
                            Debug.LogWarningFormat("A condition variable in the file {0} does not have a name value, one is required for retrieval from the varialbe repo", fileName);
                            return false;
                        }
                    }
                    else
                    {
                        if (!variable.FromRepo)
                            Debug.LogWarningFormat("A condition variable in the file {0} has name value but will be ignored as the variable is not marked to be retrieved from the variable repo", fileName);
                    }
                }


                var var1LowerType = con.Variables[0].Type?.ToLower();
                var var2LowerType = con.Variables[1].Type?.ToLower();

                // Check the comparison operator if either of the types is a string or bool
                if (var1LowerType == "string" || var1LowerType == "bool" || var2LowerType == "string" || var2LowerType == "bool")
                {
                    if (con.Comparison != "==" && con.Comparison != "!=")
                    {
                        Debug.LogWarningFormat("String and booleans can only use the == and != equality operators. Condition in file {0} is trying to use {1}", fileName, con.Comparison);
                        return false;
                    }
                }

                // String and bool are only tested against each other. This isn't quite true, normally, but it's a lot more readable to only allow string and bools to be compared against each other
                if (!con.Variables[0].FromRepo && (var1LowerType == "bool" || var1LowerType == "string") &&
                    !con.Variables[1].FromRepo && (var2LowerType == "bool" || var2LowerType == "string"))
                {
                    // Check they're compatible
                    if(var1LowerType != var2LowerType)
                    {
                        Debug.LogWarningFormat("Conversation file {0} has a condition with an unsupported variable comparison. Strings or booleans can only be tested against each other", fileName);
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // Resolve a registered conversation
    public Conversation ResolveConversation(string convoName) => _conversations.ContainsKey(convoName) ? _conversations[convoName] : null;
}
