# Dialogue System

A simple dialogue system for Unity with support for richtext tags, custom tags, and variable registration/retrieval.

## Usage

As well as the standard rich text tags, there's a few custom tags available. Hopefully more on the way soon:

  - speed - Changes the palyback speed of the text
  - send message - Used to send messages at a given point during dialogue. The UI controller is the default target if one isn't specified.
  
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
 "Let's send a message <sendmessage=messageName">. There you go, message send.
 
 // Register short with the name savedShort and the value 1
 "Let's save a short for retrieval later <registershort=savedShort>1</registershort>."
 
 // Retrieve the short value
 "Here's the short value we saved earlier <retrieveshort=savedShort>."
 
 // Also works for the other variable types
 "Time to save a string <registerstring=charactersName>The Hero</registerstring>."
 
 // Get the name back
 "Hello <retrievestring=charactersName>, long time no see."
 ```
 
 You can manually register/retrieve variables via the DialogueVarialbeRepo class.
 
 ## TODO
 - ~~Proof of concept v 0.1~~
 - Add conversations to ConversationRepo and parse/validate in real time, not just on scene start
 - Custom tag to allow for waiting/pausing during dialogue
 - Conditional starting positions e.g. start at a different dialoue if a registered variable is a certain value
 - Some sort of OnStart, OnFinish optional action
 - Would be nice to specify the target of the sendmessage tag in the dialogue JSON
 - Could always support more varaible types for registration/retrieval
 
## Would be nice list, but probably never going to happen
- A JSON conversation building
 
