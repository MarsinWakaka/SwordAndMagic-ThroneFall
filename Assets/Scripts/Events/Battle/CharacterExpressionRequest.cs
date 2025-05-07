using System;
using GameLogic.Dialogue;
using MyFramework.Utilities;
using UnityEngine;

namespace Events.Battle
{
    public class CharacterExpressionRequest : IEventArgs
    { 
        public string CharacterName;
        public CharacterExpression Expression;
        
        public Action<Sprite> OnImageReady;

        public CharacterExpressionRequest(string characterName, CharacterExpression expression, Action<Sprite> onImageReady)
        {
            CharacterName = characterName;
            Expression = expression;
            OnImageReady = onImageReady;
        }
    }
}