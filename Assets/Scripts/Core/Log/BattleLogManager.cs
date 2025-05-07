using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace Core.Log
{
    public class BattleLogManager : SceneSingleton<BattleLogManager>
    {
        public void Log(string message, Color color = default)
        {
            if (color == default) color = Color.yellow;

            // Log the message with the specified color
            // logging logic
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }
    }
}