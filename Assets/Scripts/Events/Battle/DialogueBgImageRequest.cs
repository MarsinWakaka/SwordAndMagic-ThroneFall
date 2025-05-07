using System;
using MyFramework.Utilities;
using UnityEngine;

namespace Events.Battle
{
    public class DialogueBgImageRequest : IEventArgs
    {
        public string ImageID;
        public string ImageType;

        private event Action<Sprite> OnImageLoadedCallback;

        public DialogueBgImageRequest(string imageID, string imageType, Action<Sprite> onImageLoaded)
        {
            ImageID = imageID;
            ImageType = imageType;
            OnImageLoadedCallback = onImageLoaded;
        }

        public virtual void OnImageLoaded(Sprite image)
        {
            OnImageLoadedCallback?.Invoke(image);
        }
    }
}