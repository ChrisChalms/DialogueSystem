using System;
using System.Collections.Generic;
using UnityEngine;

// The idea is that this class goes through the sentence, before the UI gets it, strips and custom tags, and makes Modification objects for the UI to check for every letter
public class TextModifications
{
    // Currently supported custom modifications
    public enum Modifications
    {
        NOT_CUSTOM,
        SPEED,
        SEND_MESSAGE,

        RETRIEVE_VARIABLE_SHORT,
        RETRIEVE_VARIABLE_INT,
        RETRIEVE_VARIABLE_LONG,
        RETRIEVE_VARIABLE_FLOAT,
        RETRIEVE_VARIABLE_BOOL,
        RETRIEVE_VARIABLE_STRING,

        REGISTER_SHORT,
        REGISTER_INT,
        REGISTER_LONG,
        REGISTER_FLOAT,
        REGISTER_BOOL,
        REGISTER_STRING
    }

    private List<SimpleModification> _modifications;
    private string _strippedSentence;

    public string Sentence => _strippedSentence;

    // Initialize
    public TextModifications(string sentence)
	{
        _modifications = new List<SimpleModification>();

        parseSentenceForCustomTags(sentence);
    }

    // Look for our custom tags, strip them out, and make the Modifications objects for the UI to look for
    // A little messy, could use a refactoring
    private void parseSentenceForCustomTags(string sentence)
    {

        // For parsing normal commands and retrievals
        var parsingCommand = false;
        var commandText = string.Empty;

        // For parsing registration tags
        var parsingComplexTag = false;
        var complexTagValue = string.Empty;
        var complexTagContent = string.Empty;

        // Info about the tag and tags added so we can revert if it's not a custom tag
        var commandStarted = 0;
        var commandCharsAdded = 0;

        _strippedSentence = string.Empty;

        for (var i = 0; i < sentence.Length; i++)
        {
            // Start parsing a tag
            if(sentence[i] == '<')
            {
                parsingCommand = true;
                parsingComplexTag = false;
                commandStarted = i - commandCharsAdded;
                commandCharsAdded++;
                continue;
            }
            // Stop parsing a tag
            else if(sentence[i] == '>')
            {
                parsingCommand = false;
                commandCharsAdded++;

                // Process add mod if it's custom
                var tempType = isCustomTag(commandText);

                // Check if retrieving variable or modifying something
                if (tempType != Modifications.NOT_CUSTOM)
                {
                    // Variable Retrieval
                    if (isRetreivalMod(tempType))
                    {
                        // Replace text with retreived variable
                        _strippedSentence += getVariableFromRepo(tempType, commandText);
                        commandCharsAdded -= (commandText.Length - 2);
                    }
                    // All other complex tags e.g. <command=value>content</command>
                    else if(isComplexTag(tempType))
                    {
                        if (!isClosingTag(commandText))
                        {
                            // Start parsing
                            parsingComplexTag = true;
                            complexTagContent = string.Empty;

                            // Check it has a name=value pattern
                            var commandSplits = getTagNameValuePair(commandText, '=');
                            if (commandSplits == null)
                                return;
                            complexTagValue = commandSplits[1];
                        }
                        // Register the variable or complex tag
                        else
                        {
                            parsingComplexTag = false;
                            if (isRegistrationTag(tempType))
                                DialogueVariableRepo.Instance.Register(complexTagValue, complexTagContent, getRegistrationTypeCode(tempType));
                            else
                                registerComplexModification(tempType, complexTagValue, complexTagContent, commandStarted);
                        }
                    }
                    else
                        registerSimpleModification(commandText, commandStarted);
                }
                // Reset as if this didn't happen
                else
                {
                    _strippedSentence += $"<{commandText}>";
                    commandCharsAdded -= (commandText.Length - 2);
                }

                commandText = string.Empty;
                continue;
            }

            // Add letter to the stripped sentence if we're not parsing
            if (!parsingCommand && !parsingComplexTag)
                _strippedSentence += sentence[i];
            // Add to the letter to the correct string
            else
            {
                if (parsingCommand)
                    commandText += sentence[i];
                else if (parsingComplexTag)
                    complexTagContent += sentence[i];
                commandCharsAdded++;
            }
        }
    }

    // Parses and registers a simple modification 
    private void registerSimpleModification(string command, int startingIndex)
    {
        var commandType = isCustomTag(command);
        if (commandType != Modifications.NOT_CUSTOM)
        {
            // Check it has a name=value pattern
            var commandSplits = getTagNameValuePair(command, '=');
            if (commandSplits == null)
                return;

            var modValue = parseModValue(commandType, commandSplits[1]);
            if (modValue != null)
                _modifications.Add(new SimpleModification { Index = startingIndex, ModType = commandType, ModificationValue = modValue });
        }
    }

    // Registers a complex modification
    private void registerComplexModification(Modifications modType, string value, string content, int startingIndex)
    {
        // Would've already check the tag is custom to get this far, the only thing that's really required is the value, should definitely be able to handle empty content
        if(string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
        {
            Debug.LogWarningFormat("Trying to parse custom tag {0}, but it has an empty value", modType);
            return;
        }

        var modValue = parseModValue(modType, value);
        if (modValue != null)
            _modifications.Add(new ComplexModification { Index = startingIndex, ModType = modType, ModificationValue = modValue, ModificationContent = content});
    }

    // Get the variable from the repo
    private object getVariableFromRepo(Modifications modType, string commandText)
    {
        var variableSplits = getTagNameValuePair(commandText, '=');
        if (variableSplits == null)
            return string.Empty;

        var variableName = variableSplits[1];
        // Retrieve short var
        if (modType == Modifications.RETRIEVE_VARIABLE_SHORT)
            return DialogueVariableRepo.Instance.RetrieveVariable<short>(variableName);
        // Retrieve int var
        else if (modType == Modifications.RETRIEVE_VARIABLE_INT)
            return DialogueVariableRepo.Instance.RetrieveVariable<int>(variableName);
        // Retrieve long var
        else if (modType == Modifications.RETRIEVE_VARIABLE_LONG)
            return DialogueVariableRepo.Instance.RetrieveVariable<long>(variableName);
        // Retrieve float var
        else if (modType == Modifications.RETRIEVE_VARIABLE_FLOAT)
            return DialogueVariableRepo.Instance.RetrieveVariable<float>(variableName);
        // Retrieve bool var
        else if (modType == Modifications.RETRIEVE_VARIABLE_BOOL)
            return DialogueVariableRepo.Instance.RetrieveVariable<bool>(variableName);
        // Retrieve string var
        else if (modType == Modifications.RETRIEVE_VARIABLE_STRING)
            return DialogueVariableRepo.Instance.RetrieveVariable<string>(variableName);

        return string.Empty;
    }

    #region Helpers

    // Return the command type if it's a custom command
    private Modifications isCustomTag(string command)
    {
        if (command.Contains("speed"))
            return Modifications.SPEED;
        else if (command.Contains("sendmessage"))
            return Modifications.SEND_MESSAGE;

        // Retrieval
        else if (command.Contains("retrieveshort"))
            return Modifications.RETRIEVE_VARIABLE_SHORT;
        else if (command.Contains("retrieveint"))
            return Modifications.RETRIEVE_VARIABLE_INT;
        else if (command.Contains("retrievelong"))
            return Modifications.RETRIEVE_VARIABLE_LONG;
        else if (command.Contains("retrievefloat"))
            return Modifications.RETRIEVE_VARIABLE_FLOAT;
        else if (command.Contains("retrievebool"))
            return Modifications.RETRIEVE_VARIABLE_BOOL;
        else if (command.Contains("retrievestring"))
            return Modifications.RETRIEVE_VARIABLE_STRING;

        // Registration
        else if (command.Contains("registershort"))
            return Modifications.REGISTER_SHORT;
        else if (command.Contains("registerint"))
            return Modifications.REGISTER_INT;
        else if (command.Contains("registerlong"))
            return Modifications.REGISTER_LONG;
        else if (command.Contains("registerfloat"))
            return Modifications.REGISTER_FLOAT;
        else if (command.Contains("registerbool"))
            return Modifications.REGISTER_BOOL;
        else if (command.Contains("registerstring"))
            return Modifications.REGISTER_STRING;

        return Modifications.NOT_CUSTOM;
    }

    // Returns whether the found tag is a closing tag - Used to detect the end of the variable parsing
    private bool isClosingTag(string command)
    {
        return command.StartsWith("/");
    }

    // Returns whether the mod is a variabel retrieval tag
    private bool isRetreivalMod(Modifications mod)
    {
        return (mod == Modifications.RETRIEVE_VARIABLE_SHORT ||
            mod == Modifications.RETRIEVE_VARIABLE_INT ||
            mod == Modifications.RETRIEVE_VARIABLE_LONG ||
            mod == Modifications.RETRIEVE_VARIABLE_FLOAT ||
            mod == Modifications.RETRIEVE_VARIABLE_BOOL ||
            mod == Modifications.RETRIEVE_VARIABLE_STRING);
    }

    // Returns whether this custom tag has a <command=value>content</command> pattern
    private bool isComplexTag(Modifications mod)
    {
        return (mod == Modifications.SEND_MESSAGE ||
            mod == Modifications.REGISTER_SHORT ||
            mod == Modifications.REGISTER_INT ||
            mod == Modifications.REGISTER_LONG ||
            mod == Modifications.REGISTER_FLOAT ||
            mod == Modifications.REGISTER_BOOL ||
            mod == Modifications.REGISTER_STRING);
    }

    // Returns whether modification is a variable registration command
    private bool isRegistrationTag(Modifications mod)
    {
        return (mod == Modifications.REGISTER_SHORT ||
            mod == Modifications.REGISTER_INT ||
            mod == Modifications.REGISTER_LONG ||
            mod == Modifications.REGISTER_FLOAT ||
            mod == Modifications.REGISTER_BOOL ||
            mod == Modifications.REGISTER_STRING);
    }

    // Return the modification's value e.g. <speed=0.1> return 0.1. Not the tags content
    private object parseModValue(Modifications modType, string commandText)
    {
        // We're already a string
        if (modType == Modifications.SEND_MESSAGE)
            return commandText;
        // Parse float
        else if (modType == Modifications.SPEED)
        {
            var floatVal = 0f;
            if (float.TryParse(commandText, out floatVal))
                return floatVal;
        }

        Debug.LogWarningFormat("Couldn't parse parameter value for {0} text modification", modType);
        return null;
    }

    // Used to return the name=value pair if possible
    private string[] getTagNameValuePair(string text, char separator)
    {
        var tempSplit = text.Split(separator);
        if (tempSplit.Length != 2)
        {
            Debug.LogWarningFormat("Cannot parse command {0}", text);
            return null;
        }

        return tempSplit;
    }

    // Returns the TypeCode of the variable we're trying to register - Can use TypeCode as we're not supporting objects
    private TypeCode getRegistrationTypeCode(Modifications modType)
    {
        if (modType == Modifications.REGISTER_SHORT)
            return TypeCode.Int16;
        else if (modType == Modifications.REGISTER_INT)
            return TypeCode.Int32;
        else if (modType == Modifications.REGISTER_LONG)
            return TypeCode.Int64;
        else if (modType == Modifications.REGISTER_FLOAT)
            return TypeCode.Single;
        else if (modType == Modifications.REGISTER_BOOL)
            return TypeCode.Boolean;
        else if (modType == Modifications.REGISTER_STRING)
            return TypeCode.String;

        return TypeCode.String;
    }

    #endregion

    // Returns the modification if there is one present
    public List<SimpleModification> GetAnyTextModsForPosition(int pos)
    {
        return _modifications.FindAll(m => m.Index == pos);
    }
}

// Class to store the simple modification information e.g. <command=value>
// A modification is considered simple if it's a simple name=value pattern with no content or closing tag
public class SimpleModification
{
    public int Index { get; set; }
    public TextModifications.Modifications ModType { get; set; }
    public object ModificationValue { get; set; }

    public T GetValue<T>()
    {
        if (ModificationValue is T)
            return (T)ModificationValue;

        throw new Exception($"Trying to cast variable of type {ModificationValue.GetType()} to type {typeof(T)}"); // Don't want to return default(T), gonna have to throw
    }
}

// Adds an extra property for the tags content e.g. <command=value>Content</command>
public class ComplexModification : SimpleModification
{
    // Always a string?
    public object ModificationContent { get; set; }

    public T GetContent<T>()
    {
        if (ModificationContent is T)
            return (T)ModificationContent;

        throw new Exception($"Trying to cast variable of type {ModificationContent.GetType()} to type {typeof(T)}"); // Don't want to return default(T), gonna have to throw
    }
}