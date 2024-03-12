using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dev
{
    using Data;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Lobby 
    {
        public void Init()
        {
            Game.SoundManager.PlayBGM(3, true);
        }
        public void Release()
        {
            Game.SoundManager.FadeGameBGMVolume(0f, 1f, () => { Game.SoundManager.StopBGM(); });
        }

        public bool TryPatchDownload(AsyncOperationHandle<IResourceLocator> initLocator)
        {
            return true;
        }

        public bool TryPatchDownload(List<IResourceLocator> initLocator)
        {
            return true;
        }
    }
}
