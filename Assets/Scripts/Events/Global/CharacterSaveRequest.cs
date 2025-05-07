using System;

namespace Events.Global
{
    public class CharacterSaveRequest
    {
        public string CharacterName;
        
        public Action<bool> OnDataReady;
    }
}