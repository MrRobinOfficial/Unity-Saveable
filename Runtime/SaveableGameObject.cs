using Newtonsoft.Json.Linq;
using UnityEngine;

using static Saveable.SaveableExtensions;
using static uJSON.uJSONExtensions;

namespace Saveable
{
    [AddComponentMenu("Tools/Saveables/Saveable [GameObject]"), DisallowMultipleComponent]
    public sealed class SaveableGameObject : MonoBehaviour, ISaveable
    {
        [System.Flags]
        public enum Flags : byte
        {
            IncludeLayer = 0x1,
            IncludeStatic = 0x2,
            [InspectorName("Include HideFlags")] IncludeHideFlags = 0x4,
            [InspectorName("Include IsActive")] IncludeIsActive = 0x8,
        }

        [SerializeField] Flags m_Flags = Flags.IncludeLayer | Flags.IncludeStatic 
            | Flags.IncludeHideFlags | Flags.IncludeIsActive;

        [System.Serializable]
        private struct GameObjectData
        {
            public bool isStatic;
            public bool isActive;
            public HideFlags hideFlags;
            public LayerMask layer;

            public GameObjectData(GameObject obj)
            {
                isStatic = obj.isStatic;
                isActive = obj.activeInHierarchy;
                hideFlags = obj.hideFlags;
                layer = obj.layer;
            }

            public override string ToString() => SerializeObject(this);
        }

        public void Load(JObject data)
        {
            var ctx = data.ToObject<GameObjectData>();

            gameObject.SetActive(ctx.isActive);
            gameObject.isStatic = ctx.isStatic;
            gameObject.layer = ctx.layer;
            gameObject.hideFlags = ctx.hideFlags;
        }

        public JObject Save() => new(new GameObjectData(gameObject));
    }
}