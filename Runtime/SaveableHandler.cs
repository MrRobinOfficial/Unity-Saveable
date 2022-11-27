using UnityEngine;

namespace Saveable
{
    [AddComponentMenu("")]
    public sealed class SaveableHandler : MonoBehaviour
    {
        private void Awake()
        {
            transform.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            hideFlags = HideFlags.NotEditable;
        }

        private void OnDestroy() => SaveableManager.Instance.SaveSnapshot();
    } 
}