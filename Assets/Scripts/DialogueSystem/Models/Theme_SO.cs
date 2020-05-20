using System;
using System.Collections.Generic;
using UnityEngine;

namespace CC.DialogueSystem
{
    [CreateAssetMenu(fileName = "Theme", menuName = "DialogueSystem/Theme", order = 1)]
    public class Theme_SO : ScriptableObject
    {
        public string Name;
        public List<DialogueTheme> ThemeSprites;
    }

    [Serializable]
    public class DialogueTheme
    {
        public string Name;
        public Sprite Sprite;
    }
}
