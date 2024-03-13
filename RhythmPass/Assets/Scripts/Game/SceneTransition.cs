using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Dev
{
    public enum SceneTypeEnum : int
    {
        LobbyScene = 0,
        WorldScene,
        LoadingScene,
    }
    public class SceneTransition 
    {
        public SceneTypeEnum CurrentSceneType { get; private set; } = SceneTypeEnum.LobbyScene;

        Coroutine _coroutine;
        public void ChangeScene(SceneTypeEnum sceneType)
        {
            if (CurrentSceneType == sceneType)
                return;
            if (null != _coroutine)
                return;
            _coroutine = Game.Instance.StartCoroutine(CoroutineLoadScene(sceneType));
        }
        IEnumerator CoroutineLoadScene(SceneTypeEnum targetScene)
        {
            SceneTypeEnum prevScene = CurrentSceneType;
            CurrentSceneType = SceneTypeEnum.LoadingScene;
            SceneManager.LoadScene((int)SceneTypeEnum.LoadingScene);
            yield return null;
            Game.Instance.OnDisabledScene(prevScene);
            yield return null;
            AsyncOperation operation = SceneManager.LoadSceneAsync((int)targetScene, LoadSceneMode.Single);
            operation.allowSceneActivation = true;

            while(false == operation.isDone)
            {
                yield return null;
            }

            CurrentSceneType = targetScene;
            Game.Instance.OnChangedScene(prevScene, targetScene);
            _coroutine = null;
        }   
    }
}