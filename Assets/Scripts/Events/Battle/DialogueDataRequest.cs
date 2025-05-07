using System;
using GameLogic.Dialogue;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class DialogueDataRequest : IEventArgs
    {
        public string DialogueId;
        
        public Action<Dialogue> OnDataReady;

        public DialogueDataRequest(string dialogueId, Action<Dialogue> onDataReady)
        {
            DialogueId = dialogueId;
            OnDataReady = onDataReady;
        }
    }
}