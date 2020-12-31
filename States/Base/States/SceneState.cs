using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Misokatsu
{
    public class SceneState : State
    {
        [Inject]
        private readonly string _sceneName = default;

        protected override async UniTask OnEnter()
        {
            await SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);

            var scene = SceneManager.GetSceneByName(_sceneName);
            SceneManager.SetActiveScene(scene);
        }

        protected override async UniTask OnExit()
        {
            await SceneManager.UnloadSceneAsync(_sceneName);
        }

        public class Factory : PlaceholderFactory<string, SceneState>
        {
        }
    }
}
