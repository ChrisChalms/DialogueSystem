using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Conversation
{
    [JsonProperty("conversation")]
    public List<Dialogue> Dialogues = new List<Dialogue>();

    // Let child classes know we've finished parsing and do any casts/prep needed
    public void FinishedParsing()
    {
        foreach (var diag in Dialogues)
            diag.FinishedParsing();
    }
}

public class Dialogue
{
    public int Id;
    public int NextId = -1;
    public string Speaker;
    public bool AutoProceed;
    public bool CanBeUsedAsStartingPoint = true;

    public List<Condition> StartConditions = new List<Condition>();
    public List<string> Sentences = new List<string>();
    public List<Option> Options = new List<Option>();

    // Let the conditions know they're ok to precast anything that needs it
    public void FinishedParsing()
    {
        foreach (var con in StartConditions)
            con.FinishedParsing();
    }

    // Evaluest the starting conditions and return whether they all passed or one or more failed
    public bool EvaluateStartingConditions()
    {
        foreach(var con in StartConditions)
        {
            if (!con.Evaluate())
                return false;
        }

        return true;
    }
}

public class Condition
{
    public List<Variable> Variables = new List<Variable>();
    public string Comparison;

    private IComparison _comparer;

    // Cast any varaibles needed and get the comparer
    public void FinishedParsing()
    {
        getComparer();
        castVariables();
    }

    // Tell the all of the conditions to pre cast the values if not in the repo
    private void castVariables()
    {
        // Cast variables
        foreach (var variale in Variables)
            variale.Cast();
    }

    // Get the right implementation of IComparison from the Comparison variable
    private void getComparer()
    {
        switch(Comparison)
        {
            case ">": _comparer = new GreaterThan(); break;
            case "<": _comparer = new LessThan(); break;
            case ">=": _comparer = new GreateOrEqualTo(); break;
            case "<=": _comparer = new LessToEqualTo(); break;
            case "==": _comparer = new EqualTo(); break;
            case "!=": _comparer = new NotEqualTo(); break;
            default: Debug.LogWarningFormat("Unsupported comparison operator {0} used", Comparison); break;
        }
    }

    // Test the variables according to the comparison string
    public bool Evaluate()
    {
        // Check the comparer
        if(_comparer == null)
        {
            Debug.LogWarning("Trying to evaluate a condition, but the comparison operator is null");
            return false;
        }

        // Get values
        var var1 = Variables[0].GetValue();
        var var2 = Variables[1].GetValue();

        if (var1 == null || var2 == null)
        {
            Debug.LogWarning("Trying to evaluate a condition but one or more of the variables are null");
            return false;
        }

        // Evaluate
        // We're using dynamics here, so be careful. We'll have to valudate types on the conversation load
        return _comparer.Execute(var1, var2);
    }
}

public class Variable
{
    public bool FromRepo = false;
    public string Name;
    public string Value;
    public string Type;

    private object _castValue;

    // Retrieve from the repo or return the precast value - if we know the type
    public T GetValue<T>()
    {
        if (FromRepo)
            return DialogueVariableRepo.Instance.RetrieveVariable<T>(Name);

        if (_castValue == null)
            Debug.LogWarningFormat("Trying to retrieve a variable value for comparison that is null");

        return (T)_castValue;
    }

    // Do we need to know the type when retrieving for evaluation?
    public object GetValue()
    {
        if (FromRepo)
            return DialogueVariableRepo.Instance.RetrieveVariable(Name);

        if (_castValue == null)
            Debug.LogWarningFormat("Trying to retrieve a variable value for comparison that is null");

        return _castValue;
    }

    // Pre cast the variables for quicker retrieval
    public void Cast()
    {
        if(!FromRepo)
        {
            switch (Type.ToLower())
            {
                case "short":
                    _castValue = short.Parse(Value);
                    break;
                case "int":
                    _castValue = int.Parse(Value);
                    break;
                case "long":
                    _castValue = long.Parse(Value);
                    break;
                case "float":
                    _castValue = float.Parse(Value);
                    break;
                case "bool": _castValue = bool.Parse(Value); break;
                case "string": _castValue = Value; break;
                default: Debug.LogWarningFormat("Unsupported type {0} using in variable", Type); break;
            }
        }
    }
}

public class Option
{
    public int NextId = -1;
    public string Text;
}

// TODO: Refactor?
#region Condition interfaces

public interface IComparison
{
    bool Execute(dynamic var1, dynamic var2);
}

// >
public class GreaterThan : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 > var2;
}

// <
public class LessThan : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 < var2;
}

// >=
public class GreateOrEqualTo : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 >= var2;
}

// <=
public class LessToEqualTo : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 <= var2;
}

// ==
public class EqualTo : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 == var2;
}

// != 
public class NotEqualTo : IComparison
{
    public bool Execute(dynamic var1, dynamic var2) => var1 != var2;
}

#endregion