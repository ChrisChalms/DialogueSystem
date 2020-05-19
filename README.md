# Dialogue System

A simple dialogue system for Unity with support for richtext tags, custom tags, and variable registration/retrieval/removal, conditional starting points, dialogue actions.
[Video of v0.68 example scene](https://youtu.be/IlXwr7wfiRM)

## Usage

Each conversation file is made up of at least one dialogue object, there are a number of different option for the objects, but the only thing that's actually required is an id and some sentences.
 
 ```javascript
 
// The simplest conversation possible
{
    "conversation":
    [
        {
            "id": 1,
            "sentences":
            [
                "The first sentence in the dialogue",
                "Some more dialogue"
            ]
        }
    ]
}
 
 ```
 Here's some more options:
 - Id - The id of the dialogue. The dialogue controller will look for the highest id to start from (if all starting condition evaluations are true)
 - Sentences - Array of the sentences that are queued
 - NextId - The Id of the dialogue to progress to after this one has finished
 - Speaker - The name of the person speaking
 - CharacterSpritesName - The name this character's sprites were registered under in the repo
 - StartingSprite - Used with CharacterSpritesName to show a character's sprite as soon as the conversation starts. You can also just have the CharacterSpritesName and the sprite "Default" will be looked for if this is blank
 - Options - An option can be used to navigate to another dialogue, perform an action (v0.65 upwards), or both 
 - CanBeUsedAsStartingPoint - Whether to consider this or not as a starting position, default is true. Useful if the conversation has to go between multiple dialogues but can't be started from half way through
 - Conditional starting positions - As of v0.45 the dialogue controller can perform conditional tests on variables to decide where to start a conversation. e.g. In the example scene, once you've accepted the quest, the wizard reacts to how much gold you have. You can test all the the variable types that available for registration/retrieval in the variable repo. The dialogue will be evaluated and only be used as a starting point if all the conditions return true. Each condition requires:
   - Comparison - The operator for the comparison to use
   - Variables - The two variables to test against each other, can either be hardcoded in the conversation file or retrieved from the repo. For a complete example of the variable testing, check out the TestConversation.json, WiseWizard.json, or HatMerchant.json in the example scene

```javascript
{
    "conversation":
    [
        // Fall back dialogue option if all starting conditions are not met
        {
            "id": 1,
            "sentences":
            [
                "Hello! We've never met before"
            ]
        },

        // Where to start if the friendliness value is above 10. The equivalent of (friendliness > 10)
        {
            "id": 2,
            "sentences":
            [
                "Ahh, hello again! Nice to see you",
                "I remember you helping me with that quest"
            ],
            "startingConditions":
            [
                {
                    "comparson": ">",
                    "variables":
                    [
                        {
                            "fromRepo": true,
                            "name": "friendliness"
                        },
                        {
                            "value": 10,
                            "type": "int"
                        }
                    ]
                }
            ]
        }

        // Where to start if the friendliness value is above 20. This will be picked before the previous test because the id is a high value. The equivalent of (friendliness > 20).
        {
            "id": 3,
            "sentences":
            [
                "Ahh, hello again! Nice to see you",
                "I remember all those times you've helped me in the past"
            ],
            "startingConditions":
            [
                {
                    "comparson": ">",
                    "variables":
                    [
                        {
                            "fromRepo": true,
                            "name": "friendliness"
                        },
                        {
                            "value": 20,
                            "type": "int"
                        }
                    ]
                }
            ]
        }
    ]
}

```

### Actions
Action are events that can be triggered from the dialogue text using the ```action``` simple tag, or passed messages or targets using the complex tags ```actionWithMessage``` and ```actionWithTarget```, or an option using the ```selectedActions``` array. An option can contain multiple actions that will be executed in the order they're written in the array. The available action types are:
  - Log
  - Log Warning
  - Log Error
  - Close Conversation
  - Send Message
  
Here's an example of a conversation with five options that are triggered in different ways:

```javascript

{
    "conversation":
    [
        // Conversation
        {
            "id": 1,
            "sentences":
            [
                "Let's perform some actions! From the dialogue <performAction=logNormal>?",
                "Or let's just load a level <action=LoadLevel>.",
                "Or load a certain level <actionWithMessage=LoadCertainLevel>NextLevel</actionWithMessage>"
            ],
            "options":
            [
                {
                    "nextId": 1,
                    "text": "Perform logging actions then restart conversation",
                    "selectedActions":[ "logNormal", "logAWarning", "logAnError" ]
                },
                {
                    "text": "End the conversation",
                    "selectedActions": [ "finishConversation" ]
                }
            ]
        },
    ],
    
    // Actions
    "actions":
    [
        // Log a message
        {
            "name": "logNormal",
            "type": "log",
            "message": "A normal log message"
        },

        // Log a warning
        {
            "name": "logAWarning",
            "type": "logWarning",
            "message": "A warning log message"
        },

        // Log an error
        {
            "name": "logAnError",
            "type": "logError",
            "message": "Log error action. Don't worry, this was supposed to happen!"
        },

        // Stop the conversation
        {
            "name": "finishConversation",
            "type": "closeConversation"
        },

        // Load the next level
        {
            "name": "LoadLevel",
            "type": "sendMessage",
            "target": "GameObject Target",
            "message": "LoadNextLevel"
        },

        // Load a particular level
        {
        	"name": "LoadCertainLevel",
            "type": "sendMessage",
            "target": "GameObject Target"
        }
    ]
}

```

### Tags

As well as the standard rich text tags, there are three different types of custom tags available, command, simple, and complex tags.
  
  #### Command Tags
  
  Commands are the simplest custom tag, they don't require and value, content, or closing tags. Currently there is only one command, ```<hideSprite>```, which will hide the character's sprite box if it's showing
  
  #### Simple Tags
  
  Simple tags add the value parameter to the command tag, below are the currently supported simple tags:
  
  - speed - Changes the palyback speed of the text e.g. ```<speed=3>```
  - removeVariable - Used to remove a registered variable from the variable repo at any point during a conversation e.g. ```<removeVariable=charactersName>```
  - wait - Waits for a secified time before continuing the conversation e.g. ```<wait=2>```
  - performAction - Execute a registered action by name e.g. ```<performAction=actionName>```
  - log - Logs a message via the DialogueLogger e.g. ```<log=A message from the conversation>```
  - logWarning - Logs a warning via the DialogueLogger e.g. ```<logWarning=A warning message from the conversation>```
  - logError - Logs an errer via the DialogueLogger e.g. ```<logError=An error message from the conversation>```
  
  #### Complex tags
  
  Complex tags contain a command, value, and content. Below are the supported complex tags:
  
  - send message - Used to send messages at a given point during dialogue to external objects. Will show a warning if the reciever is not found e.g. ```<sendMessage=recievingObject>messageToSend</sendMessage>```
  - changeSprite - Can be used to control the character's dialogue sprite, and will show the sprite box if it's not already e.g. ```<changeSprite=CharactersName>SpriteName</changeSprite>```
  - actionWithMessage - This is used to provide or override an action's message value e.g. ```<actionWithMessage=LevelLoader>LevelToLoad</actionWithMessage>```
  - actionWithTarget - This is used to add or override an action's target value e.g. ```<actionWithTarget=sendDestroyMessage>targetToDestroy</actionWithTarget>```
  - Variable registration/retrieval - There's also the ability to register/retrieve variables through dialogue. They follow the same pattern register/retrieve + variable type pattern:
    - registerShort/retrieveshort
    - registerInt/retrieveInt
    - registerLong/retrieveLong
    - registerFloat/retrieveFloat
    - registerBool/retrieveBool
    - registerString/retrieveString
 
They're all pretty straight forward to use:
 ```javascript
 // Speed
 "A normal speed sentence, <speed=3>a lot slower. <speed=1>And back to normal speed again"
 
 // Send message
 "Let's send a message<sendmessage=recievingGameObject>MessageToSend</sendmessage>. There you go, message send."
 
 // Wait
 "Wait just a second...<wait=1> Ok, carry on"
 
 // Logging
 "Let me just make a note of that real fast, <log=All good> Oh no! <logWarning=Not so good>That's not supposed to happen<logError=Not good>"
 
 // Register short with the name savedShort and the value 1
 "Let's save a short for retrieval later <registerShort=savedShort>1</registerShort>."
 
 // Retrieve the short value
 "Here's the short value we saved earlier <retrieveShort=savedShort>."
 
 // Also works for the other variable types
 "Time to save a string <registerString=charactersName>The Hero</registerString>."
 
 // Get the name back
 "Hello <retrieveString=charactersName>, long time no see."
 
 // Forget the name
 "Uhm, <removeVariable=charactersName>I've forgotten your name."
 
 // Change the character's sprite
 "Let me show you what I look like <changeSprite=charactersName>defaultSprite</changeSprite>"
 
 // Hide the character's sprite
 "Actually, I don't want you to see me..<hideSprite>."
 
 // Perform an action to load a level
 "Let's go to the next area..<action=loadNextLevel>."
 
 // Advanced actions
 "Let's load load a particular level <actionWithMessage=loadLevel>LevelToLoad</actionWithMessage>!"
 "Let's destroy things <actionWithTarget=destroyObject>Object/To/Destroy</actionWithTarget>"
 

 ```
 
You can also manually register/retrieve/remove variables via the DialogueVariableRepo class.
 
 ## TODO
 - ~~Proof of concept v 0.1~~
 - ~~Add conversations to ConversationRepo and parse/validate in real time, not just on scene start~~
 - ~~Add some sort of generic event trigger so the conversations can be added, loaded, and triggered without code~~
 - ~~Custom tag to allow for waiting during dialogue~~
 - ~~Make simple tag to remove variable from the dialogue~~
 - ~~Conditional starting positions e.g. start at a different dialoue if a registered variable is a certain value~~
 - ~~Conversation OnStart and OnFinish action~~
 - ~~Make a static logger~~
 - ~~Would be nice to specify the target of the sendmessage tag in the dialogue JSON~~
 - ~~Added helpful events to the variable repo for adding, updated, and removing~~
 - ~~Add more tag types~~
 - ~~Split tag registration into simple and complex types~~
 - ~~Add a command tag type and rework the simple and complex to inherit and add from command~~
 - ~~Make some central way for someone to regster all of a character's sprites in one place via the inspector - Used ScriptableObejcts~~
 - ~~Add a sprite repo and sprite loader, similar to the conversation loader, with loading/validation~~
 - ~~Custom tags to change, update, and hide the dialogue character's sprite~~
 - ~~Actions for options~~
 - ~~Advanced actions with tags to pass messages and targets~~
 - ~~Custom simple tag to execute an action from dialogue~~
 - ~~Logging complex tags~~
 - Add actions to more places?
 - A custom inspector to show registered variables, conversations, and sprites would be nice
 - Would like to add XML support
 - Maybe some docs or a short walkthrough
 
## Would be nice list, but probably never going to happen
- Some sort of node based conversation builder to construct the JSON files. 
