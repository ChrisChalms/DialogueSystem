using UnityEngine;

// The starting point for starting conversations and proceeding in the current conversation
public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private BaseDialogueUIController _uiController;

    private Conversation _currentConversation;
    private Dialogue _currentDialogue;
    
    private bool _inConversation;
    private int _currentSentence;

    #region MonoBehaviour

    // Initialize
    private void Start()
    {
        _uiController.DialogueController = this;
    }

    #endregion

    // Start the conversation if possible
    public void StartConversation(string convoName)
    {
        if (_inConversation)
        {
            Debug.LogWarningFormat("Trying to start conversation {0} while already in a conversation", convoName);
            return;
        }

        if(_uiController == null)
        {
            Debug.LogWarning("Trying to start a conversation, but there's not DialogueUIController assigned");
            return;
        }

        // Check if repo exists and the conversation has been loaded
        var tempoConvo = ConversationRepo.Instance?.ResolveConversation(convoName);
        if (tempoConvo == null)
        {
            Debug.LogWarningFormat("Tried to start conversation {0} but it doesn't exist is the ConversationRepo", convoName);
            return;
        }

        // Find starting point
        _currentConversation = null;
        _currentDialogue = null;
        _currentSentence = 0;
        foreach(var diag in tempoConvo.Dialogues)
        {
            if ((_currentDialogue == null || diag.Id > _currentDialogue.Id) && diag.CanBeUsedAsStartingPoint)
            {
                if(diag.StartConditions.Count > 0)
                { 
                    if (diag.EvaluateStartingConditions())
                        _currentDialogue = diag;
                }
                else
                    _currentDialogue = diag;
            }
        }

        if (_currentDialogue == null)
        {
            Debug.LogWarningFormat("Couldn't find starting point for conversation {0}", convoName);
            return;
        }

        // Start the conversation
        _inConversation = true;
        _currentConversation = tempoConvo;

        StartCoroutine(_uiController.ShowSentence(_currentDialogue.Speaker, parseSentenceForCustomTags(_currentDialogue.Sentences[_currentSentence]), _currentDialogue.AutoProceed));
    }

    // Go to the next step in the conversation e.g. Show options, go to the next dialogue in conversation, or go to the next sentence
    public void Next()
    {
        if (!_inConversation)
            return;

        if(_currentSentence >= _currentDialogue.Sentences.Count - 1)
        {
            // Check if there's options to display
            if (_currentDialogue.Options.Count != 0)
                _uiController.ShowOptions(_currentDialogue.Options);
            // Check if we need to go to another dialogue
            else if (_currentDialogue.NextId != -1)
            {
                goToDialogue(_currentDialogue.NextId);
            }
            // We've finished. Might need to check about actions here when we've finished the conversation
            else
            {
                _uiController.Close();
                _inConversation = false;
            }
        }
        else
        {
            // There's more sentences to show
            _currentSentence++;
            StartCoroutine(_uiController.ShowSentence(_currentDialogue.Speaker, parseSentenceForCustomTags(_currentDialogue.Sentences[_currentSentence]), _currentDialogue.AutoProceed));
        }
    }

    // An option was selected
    public void OptionSelected(int index)
    {
        goToDialogue(_currentDialogue.Options[index].NextId);
    }

    #region Helpers

    // Navigates to a dialogue inside the current conversation
    private void goToDialogue(int index)
    {
        // TODO: Work out actions here. e.g. set/retrieve globalvars or something
        var nextDialogue = _currentConversation.Dialogues.Find(d => d.Id == index);

        if(nextDialogue == null)
        {
            Debug.LogWarningFormat("Trying to navigate to a dialogue with the Id {0}, but there's isn't one present in the current conversation", index);
            return;
        }

        _currentDialogue = nextDialogue;
        _currentSentence = -1; // Incremented in Next()
        Next();
    }

    // Parse sentence and return TextModifications object
    private TextModifications parseSentenceForCustomTags(string sentence)
    {
        return new TextModifications(sentence);
    }

    #endregion

}
