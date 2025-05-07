using System;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class LoadDialogueEvent : IEventArgs
    {
        public readonly string DialogueId;

        public Action DialogueComplete;

        public LoadDialogueEvent(string dialogueId, Action dialogueComplete)
        {
            DialogueId = dialogueId;
            DialogueComplete = dialogueComplete;
        }
    }
}