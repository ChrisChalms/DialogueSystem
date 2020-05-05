# Changelog

All notable changes will be documented here.

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
