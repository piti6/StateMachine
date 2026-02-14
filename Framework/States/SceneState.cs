using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Misokatsu.Framework
{
    public class SceneState : State
    {
        [Inject]
        private readonly string _sceneName = default;

        protected override async UniTask OnEnterAsync()
        {
            await SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);

            var scene = SceneManager.GetSceneByName(_sceneName);
            SceneManager.SetActiveScene(scene);
        }

        protected override async UniTask OnExitAsync()
        {
            await SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}
