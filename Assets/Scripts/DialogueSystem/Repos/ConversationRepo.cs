#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    // Parses and stores all the conversations for this scene. 
    public class ConversationRepo : MonoBehaviour
    {
        [SerializeField]
        private List<TextAsset> _conversationsToLoad;

        private Dictionary<string, Conversation> _conversations;
        private IDeserializer _deserializer;

        public static ConversationRepo Instance { get; private set; }

        #region MonoBehaviour

        // Apply singleton
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            new VariableRepo();
            new SpriteRepo();

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
                RegisterConversation(file.name, file);
        }

        // Register and validate a conversation in real-time
        public void RegisterConversation(string name, TextAsset file)
        {
            // Can't load if there's no name
            if (string.IsNullOrEmpty(name))
            {
                DialogueLogger.LogError("Cannot register a conversation without a name");
                return;
            }

            // Can't load if there's no conversation
            if (file == null)
            {
                DialogueLogger.LogError("Cannot register a conversation without a conversation file");
                return;
            }

            var tempObject = _deserializer.Deserialize<Conversation>(file.text);
            if (tempObject == null)
            {
                DialogueLogger.LogError($"There was an error deserializing file {file.name}");
                return;
            }

            tempObject.PreValidation();
            if (valdateConversation(tempObject, name))
            {
                if (_conversations.ContainsKey(name))
                    DialogueLogger.LogWarning($"Conversation {name} already registered, overwritting");

                _conversations[name] = tempObject;
                tempObject.FinishedParsing();
            }
        }

        // Do simple validation on the conversation e.g. That it's not empy, that all nextIds go somewhere, things like that
        // TODO: Move all validation code to the relevant class in the Conversation.cs
        private bool valdateConversation(Conversation tempConversation, string fileName)
        {
            var dialogueIds = new List<int>();
            var actionNames = new List<string>();

            // Get all the action names for this conversation
            foreach (var action in tempConversation.Actions)
            {
                // Check we at least have a name and type
                if (string.IsNullOrEmpty(action.Name) || string.IsNullOrWhiteSpace(action.Name) ||
                    string.IsNullOrEmpty(action.Type) || string.IsNullOrWhiteSpace(action.Type))
                {
                    DialogueLogger.LogError($"An action in file {fileName} has an empty name or type value");
                    return false;
                }

                // Check if it already exists
                if (actionNames.Contains(action.Name))
                {
                    DialogueLogger.LogError($"An action in file {fileName} does not have a unique name");
                    return false;
                }
                actionNames.Add(action.Name);

                // Check action type and add name to the list
                // Errors in DialogueAction class
                if (!action.GetActionType())
                    return false;

                // An action's message and target can be set in real time via the dialogue, so it'll have to be verified in the DialogueController.
                // Verifiy all of the essential values here
            }

            // Check number of dialogues
            if (tempConversation.Dialogues.Count == 0)
            {
                DialogueLogger.LogError($"Empty conversation in file {fileName}");
                return false;
            }

            // Check dialogues sentences and populate Id list
            foreach (var diag in tempConversation.Dialogues)
            {
                // Check sentence count
                if (diag.Sentences.Count == 0)
                {
                    DialogueLogger.LogError($"Dialogue in file {fileName} has no sentences");
                    return false;
                }

                // Check sentence for empty
                foreach (var sentence in diag.Sentences)
                {
                    if (string.IsNullOrEmpty(sentence))
                    {
                        DialogueLogger.LogError($"Empty sentence in file {fileName}");
                        return false;
                    }
                }

                // Check CharacterSpriteNames and StartingSprites
                // Check the there's a CharacterSpriteName if we've sepecified a StartingSprite
                if (!string.IsNullOrEmpty(diag.StartingSprite) && string.IsNullOrEmpty(diag.CharacterSpritesName))
                {
                    DialogueLogger.LogError($"Dialogue in file {fileName} has a startingSprite but no characterSpritesName is specified");
                    return false;
                }

                // Check the OnFinishedActionNames
                // Loop through all of the action and make sure they exist
                foreach (var action in diag.OnFinishedActionNames)
                {
                    if (!actionNames.Contains(action))
                    {
                        DialogueLogger.LogError($"An dialogues OnFinishActions in file {fileName} attemps to call an action with the name {action} that doesn't exist.");
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
                if (diag.NextId != -1)
                {
                    if (!dialogueIds.Contains(diag.NextId))
                    {
                        DialogueLogger.LogError($"Dialoge in file {fileName} references a dialogue by id {diag.NextId} that doesn't exist");
                        return false;
                    }
                }

                // Check option texts and nextIds
                foreach (var option in diag.Options)
                {
                    // Check if any of the option has an action
                    // loop through all of the action and make sure they exist
                    foreach (var action in option.SelectedActionNames)
                    {
                        if (!actionNames.Contains(action))
                        {
                            DialogueLogger.LogError($"An option's selectedAction in file {fileName} attemps to call an action with the name {action} that doesn't exist.");
                            return false;
                        }
                    }

                    // Check text
                    if (string.IsNullOrEmpty(option.Text))
                    {
                        DialogueLogger.LogError($"Empty option in file {fileName}");
                        return false;
                    }

                    // Check the next dialogue exists if there's no action
                    if (option.SelectedActionNames.Count == 0 && (!dialogueIds.Contains(option.NextId) || option.NextId == -1))
                    {
                        DialogueLogger.LogError($"An option in file {fileName} has a nextId {option.NextId} that doesn't exist. If an option hasn't got an action, it needs a nextId");
                        return false;
                    }
                }

                // Check Conditions and Variables
                foreach (var con in diag.StartConditions)
                {
                    // Can only test 2 varaibles against each other. When a conversation is picked all conditions must evaluate to true, so if more tests are needed just stack conditions
                    if (con.Variables.Count != 2)
                    {
                        DialogueLogger.LogError($"A condition in file {fileName} does not contains {con.Variables.Count} variables, must have 2");
                        return false;
                    }

                    // Check the type, value, and name individually
                    foreach (var variable in con.Variables)
                    {
                        // Check there's a value if we're not retrieving from the repo
                        if (string.IsNullOrEmpty(variable.Value))
                        {
                            if (!variable.FromRepo)
                            {
                                DialogueLogger.LogError($"A condition variable in file {fileName} has a conditional varialbe without a value. One is required if not retrieving from the variable repo");
                                return false;
                            }
                        }
                        else
                        {
                            if (variable.FromRepo)
                                DialogueLogger.LogWarning($"A condition variable in file {fileName} has a value it will be ignored as the variable is marked to be retrieved from the variable repo");
                        }

                        // A type is required if we're not retrieving from the repo
                        if (string.IsNullOrEmpty(variable.Type))
                        {
                            if (!variable.FromRepo)
                            {
                                DialogueLogger.LogError($"A condition in file {fileName} has a conditional variable without a type. One is required if not retrieving from the variable repo");
                                return false;
                            }
                        }
                        else
                        {
                            if (variable.FromRepo)
                                DialogueLogger.LogWarning($"A condition variable in file {fileName} has a type but will be ignored as the variable is marked to be retrieved from the variable repo");
                        }

                        // Check there's a name if we're retrieving from the repo
                        if (string.IsNullOrEmpty(variable.Name))
                        {
                            if (variable.FromRepo)
                            {
                                DialogueLogger.LogError($"A condition variable in the file {fileName} does not have a name value, one is required for retrieval from the varialbe repo");
                                return false;
                            }
                        }
                        else
                        {
                            if (!variable.FromRepo)
                                DialogueLogger.LogWarning($"A condition variable in the file {fileName} has name value but will be ignored as the variable is not marked to be retrieved from the variable repo");
                        }
                    }


                    var var1LowerType = con.Variables[0].Type?.ToLower();
                    var var2LowerType = con.Variables[1].Type?.ToLower();

                    // Check the comparison operator if either of the types is a string or bool
                    if (var1LowerType == "string" || var1LowerType == "bool" || var2LowerType == "string" || var2LowerType == "bool")
                    {
                        if (con.Comparison != "==" && con.Comparison != "!=")
                        {
                            DialogueLogger.LogError($"String and booleans can only use the == and != equality operators. Condition in file {fileName} is trying to use {con.Comparison}");
                            return false;
                        }
                    }

                    // String and bool are only tested against each other. This isn't quite true, normally, but it's a lot more readable to only allow string and bools to be compared against each other
                    if (!con.Variables[0].FromRepo && (var1LowerType == "bool" || var1LowerType == "string") &&
                        !con.Variables[1].FromRepo && (var2LowerType == "bool" || var2LowerType == "string"))
                    {
                        // Check they're compatible
                        if (var1LowerType != var2LowerType)
                        {
                            DialogueLogger.LogError($"Conversation file {fileName} has a condition with an unsupported variable comparison. Strings or booleans can only be tested against each other");
                            return false;
                        }
                    }
                }

                // Warn if autoProceed is false and the conversation type is background, we'll autoproceed anyway
                if (tempConversation.ConversationType == Conversation.Types.BACKGROUND)
                {
                    if (!diag.AutoProceed)
                        DialogueLogger.LogWarning($"Dialogue in the file {fileName} is of type background and autoProceed is false. Background conversations always autoproceed");
                }
            }

            return true;
        }

        // Resolve a registered conversation
        public Conversation RetrieveConversation(string convoName) => _conversations.ContainsKey(convoName) ? _conversations[convoName] : null;
    }
}