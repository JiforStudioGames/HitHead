using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.UI.Core
{
    public abstract class UIScreenView : MonoBehaviour, IView
    {
        public bool ActiveState => gameObject.activeSelf;
		
        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            await UniTask.CompletedTask;
        }
        public virtual async UniTask Hide()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }
    }
}