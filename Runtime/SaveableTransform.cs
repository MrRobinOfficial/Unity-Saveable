using Newtonsoft.Json.Linq;
using UnityEngine;
using static Saveable.SaveableExtensions;
using static uJSON.uJSONExtensions;

namespace Saveable
{
    [AddComponentMenu("Tools/Saveables/Saveable [Transform]"), DisallowMultipleComponent]
    public sealed class SaveableTransform : MonoBehaviour, ISaveable
    {
        [System.Flags]
        public enum TransformConstraints : ushort
        {
            PositionX = 1 << 0,
            PositionY = 1 << 1,
            PositionZ = 1 << 2,
            RotationX = 1 << 3,
            RotationY = 1 << 4,
            RotationZ = 1 << 5,
            ScaleX = 1 << 6,
            ScaleY = 1 << 7,
            ScaleZ = 1 << 8,
            Position = PositionX | PositionY | PositionZ,
            Rotation = RotationX | RotationY | RotationZ,
            Scale = ScaleX | ScaleY | ScaleZ,
        }

        [SerializeField, TransformFlags] TransformConstraints m_Flags = TransformConstraints.Position | TransformConstraints.Rotation | TransformConstraints.Scale;

        [System.Serializable]
        private struct TransformData
        {
            public TransformData(Transform transform)
            {
                //if (transform.parent == null)
                //    parentName = string.Empty;
                //else
                //    parentName = transform.parent.name;

                position = transform.position;
                rotation = transform.eulerAngles;
                scale = transform.localScale;
            }

            //public string parentName;
            public ReadonlyVector3 position;
            public ReadonlyVector3 rotation;
            public ReadonlyVector3 scale;

            public override string ToString() => SerializeObject(this);
        }

        public void Load(JObject data)
        {
            var ctx = data.ToObject<TransformData>();

            //if (m_Flags.HasFlag(Flags.Parent) && !string.IsNullOrEmpty(ctx.parentName))
            //    transform.SetParent(GameObject.Find(ctx.parentName).transform);

            if (m_Flags.HasFlag(TransformConstraints.Position))
                transform.position = ctx.position;

            if (m_Flags.HasFlag(TransformConstraints.Rotation))
                transform.eulerAngles = ctx.rotation;

            if (m_Flags.HasFlag(TransformConstraints.Scale))
                transform.localScale = ctx.scale;
        }

        public JObject Save() => new(new TransformData(transform));
    }
}