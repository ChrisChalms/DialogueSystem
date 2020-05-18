# Changelog

All notable changes will be documented here.

## [v0.65](https://github.com/ChrisChalms/DialogueSystem/commit/d2a3f36e7fcf60925e08059c1503e2d676ef6462) - 18/05/2020

### Added
- Actions: log, log warning, log error, close conversation, and send message
- The ```selectedActions``` array to options
- ```<performAction=actionName>``` simple tag
- Extra validation to the conversation loader
- ```DialogueAction``` class to the conversation.cs

### Changed
- Moved the core dialogue system into the new CC.DialogueSystem namespace
- Moved everything not part of the core dialogue system into the ExampleScene folders
- Example scene testing conversation now implements actions

## [v0.6](https://github.com/ChrisChalms/DialogueSystem/commit/db445328f95c5e2931bf6fdb0390e3864699ca98) - 14/05/2020

### Added
- ScriptableObject to store all of a character's sprites in one palce
- Sprite repo with loader
- Character sprites to the exmaple scene
- Command tags
- Sprite box to dialogue UI
- hideSprite command
- changeSprite complex command

### Changed
- Simple and complex tags now inherit from the simplest command tag
- Changed the example scene to include the new sprite functionality

## [v0.55](https://github.com/ChrisChalms/DialogueSystem/commit/8e471d07396fa49c4b74faabb75c4cea4a40952e) - 12/05/2020

### Added
- Simple wait tag
- Simple removeVariable tag
- Acions to the DialogueController for conversation starting and ending
- Static logger added
- New functionality testing conversation to the example game

### Changed
- Changed the variable repo's events to actions

## [v0.5](https://github.com/ChrisChalms/DialogueSystem/commit/bd4b0518c8886cb546ac923c146cdb00e08d9df6) - 10/05/2020

### Added
 - Functionality to remove a registered variable from the variable repo
 - Variable repo events for when a variable is registered, updated, and removed
 
### Changed
- Surpressed the 649 about a variable never getting intialzed when using SerializeField to set in the editor

### Removed
- Testing code in the conversation repo Update function

## [v0.45](https://github.com/ChrisChalms/DialogueSystem/commit/8607cfed96beebbe56d30885864457010a25a54c) - 05/05/2020

### Added
- Functionality to store, retrieve, and evaluate variables from the dialogue, and use those evaluations to choose where to start a conversation
- A new example scene to show off the newer features of the dialogue system
  - New player controller, collision detection, and animation controller
  - Added treasure and coin items
  - Two new NPCs with conversation files

### Changed
- Heavily modified the conversation model file to support the new variable and condition functionality
- Added validation for the new conversation model
- Variable repo returns default(T) instead of throwing exceptions
- Dialogue controller now looks for the highest dialogue if for a starting point
- Reorganised the example scene assets to to be grouped together under one folder

### Removed
- Old example scene
- Old characters and their conversation files
- Player controller
- Camera follow script
- Example message recieving scripts

## [v.35](https://github.com/ChrisChalms/DialogueSystem/commit/c1bf93412d2825a87266a2af349a37fac0e1f23a) - 02/05/2020

### Changed
- Variable repo not allows overwritting of existing variables.

## [v0.3](https://github.com/ChrisChalms/DialogueSystem/commit/26b7bbb7f7b2e8d54288ec38f70f8e87bfb77e1a) - 01/05/2020

### Added
- Simple and complex modification types

### Changed
- Changed the sendmessage tag to a complex modification, allowing you to specify a target object to be found for the message. e.g. <sendmessage=recievingGameObject>MessageToSend</sendmessage>
- Modification registration and parsing is a little easier now
- Example scene to show the changes of v0.3

### Removed
- The normal modification class. Not needed after introducing the simple and complex types

## [v0.2](https://github.com/ChrisChalms/DialogueSystem/commit/25fdb88b4f08347c3f379087a1ab0ca91152c52d) - 29/04/2020

### Added
- Conversation loading script so conversation can be added to objects directly then loaded in realtime, either on start automatically, or manually
- Event Triggers helper. Allows you to add events on trigger enter and exit and specifiy whether they're one-shot or not. Not specific to the dialogue system, but used to trigger conversation loading without code in the example scene
- Added some more examples to the example scene

### Changed
- Conversation Repo now supports adding and validating conversations in realtime
- Organised the script files


## [v0.1](https://github.com/ChrisChalms/DialogueSystem/commit/00f8ed7e416f7e0c7a29639a916e32a0e7854df7) - 23/04/2020

### Added
- Intial commit, more of a proof of concept
- Support for standard rich text tags, speed and sendmessage custom tags, and register/retrieval of variables
- Simple Example scene
