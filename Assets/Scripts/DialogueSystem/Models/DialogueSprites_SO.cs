using System;
using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    [CreateAssetMenu(fileName = "Character Sprites", menuName = "DialogueSystem/Character Sprites", order = 2)]
    public class DialogueSprites_SO : ScriptableObject
    {
        public string CharactersName;
        public List<DialogueCharacterSprite> CharacterSprites;
    }

    [Serializable]
    public class DialogueCharacterSprite
    {
        public string Name;
        public Sprite Sprite;
    }
}