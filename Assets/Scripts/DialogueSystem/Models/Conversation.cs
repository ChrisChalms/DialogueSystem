using Newtonsoft.Json;
using System.Collections.Generic;

public class Conversation
{
    [JsonProperty("conversation")]
    public List<Dialogue> Dialogues = new List<Dialogue>();
}

public class Dialogue
{
    public int Id;
    public int NextId = -1;
    public string Speaker;
    public bool AutoProceed;

    public List<string> Sentences = new List<string>();
    public List<Option> Options = new List<Option>();
}

public class Option
{
    public int NextId = -1;
    public string Text;
}
