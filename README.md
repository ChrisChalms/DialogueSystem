# Dialogue System

A simple dialogue system for Unity with support for richtext tags, custom tags, and variable registration/retrieval.
[Video of v0.3 example scene](https://youtu.be/VmKpukNvjMk)

## Usage

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
 
 You can manually register/retrieve variables via the DialogueVarialbeRepo class.
 
 ## TODO
 - ~~Proof of concept v 0.1~~
 - ~~Add conversations to ConversationRepo and parse/validate in real time, not just on scene start~~
 - ~~Add some sort of generic event trigger so the conversations can be added, loaded, and triggered without code~~
 - Custom tag to allow for waiting/pausing during dialogue
 - Conditional starting positions e.g. start at a different dialoue if a registered variable is a certain value
 - Some sort of OnStart, OnFinish optional action
 - ~~Would be nice to specify the target of the sendmessage tag in the dialogue JSON~~
 - Could always support more varaible types for registration/retrieval
 - ~~Add more tag types~~ - Split tag registration into simple and complex types
 - Would like to add XML support
 - Maybe some docs or a short walkthrough
 
## Would be nice list, but probably never going to happen
- Some sort of conversation builder. Currently you have to use an editor to make the JSON files at the moment
 
