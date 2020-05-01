using System;
using System.Collections.Generic;
using UnityEngine;

// Repo to store all of the dialogue variable. Handles the registration/retrieval
public class DialogueVariableRepo
{
    private static DialogueVariableRepo _instance;

    private Dictionary<string, DialogueVariable> _variables;

    public static DialogueVariableRepo Instance => _instance;

    // Initialize
    public DialogueVariableRepo()
    {
        if (_instance == null)
            _instance = this;

        _variables = new Dictionary<string, DialogueVariable>();
    }

    #region Registration/Retrieval

    // Register a new variable if it doesn't exist
    public void Register<T>(string name, T variableValue)
    {
        // Update Existing
        if (_variables.ContainsKey(name))
            _variables[name].Value = variableValue;
        // Add
        else
            _variables.Add(name, new DialogueVariable { Value = variableValue });
    }

    // Registration through the dialogue system will always be a string, preparse as variableType and register
    public void Register(string name, string variable, TypeCode variableType)
    {
        // We're already of type string
        if (variableType == TypeCode.String)
        {
            Register(name, variable);
            return;
        }

        // I don't like this, there's got to be a better way of doing this
        try
        {
            object castVariable = null;
            switch (variableType)
            {
                case TypeCode.Int16:
                    castVariable = short.Parse(variable);
                    break;
                case TypeCode.Int32:
                    castVariable = int.Parse(variable);
                    break;
                case TypeCode.Int64:
                    castVariable = long.Parse(variable);
                    break;
                case TypeCode.Single:
                    castVariable = float.Parse(variable);
                    break;
                case TypeCode.Boolean:
                    castVariable = bool.Parse(variable);
                    break;
            }

            Register(name, castVariable);
        }
        catch(Exception e)
        {
            Debug.LogWarningFormat("Error registering variable {0} to the type {1}. Error message: {2}", variable, variableType.ToString(), e.Message);
        }
    }

    // Return the variable if it exists
    public T RetrieveVariable<T>(string name)
    {
        if (!_variables.ContainsKey(name))
            throw new Exception($"Trying to retrieve the variable {name} but it hasn't been registered"); // Don't want to return default(T), gonna have to throw
        else
            return _variables[name].GetValue<T>();
    }

    #endregion
}

// Internal class to store the variables and handle the retrieval
internal class DialogueVariable
{
    public object Value { get; set; }

    public T GetValue<T>()
    {
        if(Value is T)
            return (T)Value;

        throw new Exception($"Trying to cast variable of type {Value.GetType()} to type {typeof(T)}"); // Don't want to return default(T), gonna have to throw
    }
}