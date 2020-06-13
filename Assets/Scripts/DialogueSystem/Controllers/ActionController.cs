using UnityEngine;

namespace CC.DialogueSystem
{
    public abstract class ActionController
    {
        // Find the action in the conversation then pass it on for validating and execution
        public static void PerformAction(Conversation conversation, string actionName)
        {
            var action = getAction(conversation, actionName);

            if (action != null)
                performAction(action);
            else
                DialogueLogger.LogError("Cannot find the selectedAction for the option selected. Skipping action");
        }

        // Find the action fill in the message then pass it on for validation and execution
        public static void PerformActionWithMessage(Conversation conversation, string actionName, string message)
        {
            var action = getAction(conversation, actionName);

            if (action != null)
            {
                action.Message = message;
                performAction(action);
            }
            else
                DialogueLogger.LogError("Cannot find the selectedAction for the option selected. Skipping action");
        }

        // Find the action fill in the target then pass it on for validation and execution
        public static void PerformActionWithTarget(Conversation conversation, string actionName, string target)
        {
            var action = getAction(conversation, actionName);

            if (action != null)
            {
                action.Target = target;
                performAction(action);
            }
            else
                DialogueLogger.LogError("Cannot find the selectedAction for the option selected. Skipping action");
        }

        // Validate then execute
        private static void performAction(DialogueAction action)
        {
            if (!validateAction(action))
                return;

            switch (action.ActionType)
            {
                case DialogueAction.Types.LOG: DialogueLogger.Log(action.Message); break;
                case DialogueAction.Types.LOG_WARNING: DialogueLogger.LogWarning(action.Message); break;
                case DialogueAction.Types.LOG_ERROR: DialogueLogger.LogError(action.Message); break;

                case DialogueAction.Types.CLOSE_CONVERSATION: DialogueController.Instance?.StopCurrentConversation(); break;

                case DialogueAction.Types.SEND_MESSAGE:
                    var targetObject = GameObject.Find(action.Target);

                    if (targetObject == null)
                    {
                        DialogueLogger.LogError($"Trying to execute a send message action, but GameObject {action.Target} was not found. Skipping action");
                        return;
                    }
                    targetObject.SendMessage(action.Message, SendMessageOptions.DontRequireReceiver);
                    break;

                case DialogueAction.Types.CHANGE_THEME:
                    DialogueController.Instance?.ChangeTheme(action.Message);
                    break;

                case DialogueAction.Types.CLOSE_BG_CONVERSATIONS:
                    BackgroundDialogueController.Instance?.CloseConversations();
                    break;

                case DialogueAction.Types.START_BG_CONVERSATION:
                    BackgroundDialogueController.Instance?.StartConversation(action.Message);
                    break;

                default:
                    DialogueLogger.LogError($"Action with the name {action.Name} has na unrecognised action type {action.ActionType}. The action type loaded from the conversation JSON is {action.Type}. Skipping action");
                    break;
            }
        }

        // Validate the action
        private static bool validateAction(DialogueAction action)
        {
            // Check we've got all the pieces we neewd
            switch (action.ActionType)
            {
                case DialogueAction.Types.LOG:
                case DialogueAction.Types.LOG_WARNING:
                case DialogueAction.Types.LOG_ERROR:
                    // All we need's a message
                    if(string.IsNullOrEmpty(action.Message) || string.IsNullOrWhiteSpace(action.Message))
                    {
                        DialogueLogger.LogError($"An action with the name {action.Name} is trying to log with an empty message value");
                        return false;
                    }
                    break;

                case DialogueAction.Types.SEND_MESSAGE:
                    // We need a target and a message
                    if (string.IsNullOrEmpty(action.Message) || string.IsNullOrWhiteSpace(action.Message))
                    {
                        DialogueLogger.LogError($"An action with the name {action.Name} is trying to send a message with an empty message value");
                        return false;
                    }
                    if (string.IsNullOrEmpty(action.Target) || string.IsNullOrWhiteSpace(action.Target))
                    {
                        DialogueLogger.LogError($"An action with the name {action.Name} is trying to send a message with an empty target value");
                        return false;
                    }
                    break;

                case DialogueAction.Types.CHANGE_THEME:
                    // All we need's a message
                    if (string.IsNullOrEmpty(action.Message) || string.IsNullOrWhiteSpace(action.Message))
                    {
                        DialogueLogger.LogError($"An action with the name {action.Name} is trying to change the theme with an empty message value");
                        return false;
                    }
                    break;

                case DialogueAction.Types.START_BG_CONVERSATION:
                    // Just need message, the name of the conversation to start
                    if(string.IsNullOrEmpty(action.Message) || string.IsNullOrWhiteSpace(action.Message))
                    {
                        DialogueLogger.LogError($"An action with the name {action.Name} is trying to start a background conversation with an empty message value");
                        return false;
                    }
                    break;
            }

            return true;
        }

        #region Helpers

        // Return the action if found
        private static DialogueAction getAction(Conversation conversation, string actionName) => conversation.Actions.Find(a => a.Name == actionName);

        #endregion
    }
}