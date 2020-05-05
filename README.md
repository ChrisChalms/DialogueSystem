# Dialogue System

A simple dialogue system for Unity with support for richtext tags, custom tags, and variable registration/retrieval.
[Video of v0.45 example scene](https://youtu.be/Ppc4LU24weM)

## Usage

### Conversation options
 
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
 - Options - An array of objects containing a NextId and Text property.
 - CanBeUsedAsStartingPoint - Whether to consider this or not as a starting position, default is true. Useful if the conversation has to go between multiple dialogues but can't be started from half way through
 - Conditional starting positions - As of v0.45 the dialogue controller can perform conditional tests on variables to decide where to start a conversation. e.g. In the example scene, once you've accepted the quest, the wizard reacts to how much gold you have. You can test all the the variable types that available for registration/retrieval in the variable repo. The dialogue will be evaluated and only be used as a starting point if all the conditions return true. Each condition requires:
   - Comparison - The operator for the comparison to use
   - Variables - The two variables to test against each other, can either be hardcoded in the conversation file or retrieved from the repo. For a complete example of the variable testing, check out the WiseWizard.json or HatMerchant.json in the example scene

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

### Tags

As well as the standard rich text tags, there are two different types of custom tags available, simple and complex. Simple tags are single parameter tags that don't require a closing tag, like the speed tag below. e.g <command=value>. Complex tags allow for a tag to have content, such as the sendmessage tag below. e.g. <command=value>content</command>

  - speed - Changes the palyback speed of the text
  - send message - Used to send messages at a given point during dialogue to external objects. Will show a warning if the reciever is not found
  
  - Variable registration/retrieval - There's also the ability to register/retrieve variables through dialogue. they follow the same pattern register/retrieve + variable type pattern
    - registershort/retrieveshort
    - registerint/retrieveint
    - registerlong/retrievelong
    - registerfloat/retrievefloat
    - registerbool/retrievebool
    - registerstring/retrievestring
 
 They're all pretty straight forward to use:
 ```javascript
 // Speed
 "A normal speed sentence, <speed=3>a lot slower. <speed=1>And back to normal speed again"
 
 // Send message
 "Let's send a message<sendmessage=recievingGameObject>MessageToSend</sendmessage>. There you go, message send."
 
 // Register short with the name savedShort and the value 1
 "Let's save a short for retrieval later <registershort=savedShort>1</registershort>."
 
 // Retrieve the short value
 "Here's the short value we saved earlier <retrieveshort=savedShort>."
 
 // Also works for the other variable types
 "Time to save a string <registerstring=charactersName>The Hero</registerstring>."
 
 // Get the name back
 "Hello <retrievestring=charactersName>, long time no see."
 ```
 
 You can also manually register/retrieve variables via the DialogueVarialbeRepo class, see the example scene for an implementation
 
 ## TODO
 - ~~Proof of concept v 0.1~~
 - ~~Add conversations to ConversationRepo and parse/validate in real time, not just on scene start~~
 - ~~Add some sort of generic event trigger so the conversations can be added, loaded, and triggered without code~~
 - Custom tag to allow for waiting/pausing during dialogue
 - ~~Conditional starting positions e.g. start at a different dialoue if a registered variable is a certain value~~
 - Some sort of OnStart, OnFinish optional action
 - ~~Would be nice to specify the target of the sendmessage tag in the dialogue JSON~~
 - Could always support more varaible types for registration/retrieval
 - ~~Add more tag types~~ - Split tag registration into simple and complex types
 - Would like to add XML support
 - Maybe some docs or a short walkthrough
 
## Would be nice list, but probably never going to happen
- Some sort of conversation builder. Currently you have to use an editor to make the JSON files at the moment
 
