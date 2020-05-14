using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Character Sprites", menuName="DialogueSystem/Character Sprites")]
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
