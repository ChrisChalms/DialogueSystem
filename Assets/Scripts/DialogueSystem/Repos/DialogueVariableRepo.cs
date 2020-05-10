using System;
using System.Collections.Generic;
using UnityEngine;

// Repo to store all of the dialogue variable. Handles the registration/retrieval
public class DialogueVariableRepo
{
    // Events
    public event EventHandler<VariableEventArgs> VariableRegistered;
    public event EventHandler<VariableEventArgs> VariableUpdated;
    public event EventHandler<VariableEventArgs> VariableRemoved;

    private Dictionary<string, DialogueVariable> _variables;

    public static DialogueVariableRepo Instance { get; private set; }

    // Initialize
    public DialogueVariableRepo()
    {
        if (Instance == null)
            Instance = this;

        _variables = new Dictionary<string, DialogueVariable>();
    }

    #region Registration, Retrieval, and Removal

    // Register a new variable if it doesn't exist
    public void Register<T>(string name, T variableValue)
    {
        // Update Existing
        if (_variables.ContainsKey(name))
        {
            _variables[name].Value = variableValue;
            InvokeEvent(VariableUpdated, name);
        }
        // Add
        else
        {
            _variables.Add(name, new DialogueVariable { Value = variableValue });
            InvokeEvent(VariableRegistered, name);
        }
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

        // I don't like this, there's got to be a better way of doing it
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
    public T Retrieve<T>(string name)
    {
        // I don't like returning a default like this, but it's better than an exception
        if (!_variables.ContainsKey(name))
            return default(T);
        else
            return _variables[name].GetValue<T>();
    }

    // Get the variable if it exists and we don't care about the type
    public object Retrieve(string name)
    {
        if (!_variables.ContainsKey(name))
        {
            Debug.LogWarningFormat("Trying to retrieve the variable {0} but it hasn't been registered", name);
            return null;
        }
        else
            return _variables[name].Value;
    }

    // Remove the variable if it exists
    public void Remove(string name)
    {
        if (!_variables.ContainsKey(name))
        {
            Debug.LogWarningFormat("Trying to remove a variable {0} but it hasn't been registered", name);
            return;
        }
        else
        {
            _variables.Remove(name);
            InvokeEvent(VariableRemoved, name);
        }
    }

    #endregion

    #region Helpers

    // Invoke the supplied event if we can
    public void InvokeEvent(EventHandler<VariableEventArgs> handler, string key)
    {
        handler?.Invoke(this, new VariableEventArgs { VariableKey = key });
    }

    #endregion
}

// Internal class to store the variables and handle the retrieval
internal class DialogueVariable
{
    public object Value { get; set; }

    // Get the value as a type
    public T GetValue<T>()
    {
        if(Value is T)
            return (T)Value;

        // I don't like returning a default like this, but it's better than an exception
        return default(T);
    }

}

// Class to hold the key of the variable changed/added
public class VariableEventArgs : EventArgs
{
    public string VariableKey { get; set; }
}