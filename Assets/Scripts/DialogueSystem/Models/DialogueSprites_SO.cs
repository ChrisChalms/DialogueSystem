using System;
using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    [CreateAssetMenu(fileName = "Character Sprites", menuName = "DialogueSystem/Character Sprites")]
    public class DialogueSprites_SO : ScriptableObject
    {
        public List<DialogueCharacterSprite> CharacterSprites;
    }

    [Serializable]
    public class DialogueCharacterSprite
    {
        public string Name;
        public Sprite Sprite;
    }
}