using Newtonsoft.Json.Linq;
using UnityEngine;
using static uJSON.uJSONExtensions;

namespace Saveable.Samples
{
    [AddComponentMenu("Tools/Saveables/Samples/Saveable Player [Sample]"), DisallowMultipleComponent]
    public class SaveablePlayer : MonoBehaviour, ISaveable
    {
        public void Load(JObject data)
        {
            name = data
                .GetValue("Name")
                .ToObject<string>();

            transform.position = data
                .GetValue("Position")
                .ToObject<Vector3>();

            transform.eulerAngles = data
                .GetValue("Rotation")
                .ToObject<Vector3>();

            transform.localScale = data
                .GetValue("Scale")
                .ToObject<Vector3>();
        }

        public JObject Save()
        {
            JTokenWriter writer = new JTokenWriter();
            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(name);

            writer.WritePropertyName("Position");
            writer.WriteValue(SerializeObject(transform.position));

            writer.WritePropertyName("Rotation");
            writer.WriteValue(SerializeObject(transform.rotation));

            writer.WritePropertyName("Scale");
            writer.WriteValue(SerializeObject(transform.localScale));

            writer.WritePropertyName("worldToLocalMatrix");
            writer.WriteValue(SerializeObject(transform.worldToLocalMatrix));

            writer.WritePropertyName("gameObject");
            writer.WriteValue(SerializeObject(transform.gameObject));

            writer.WritePropertyName("hideFlags");
            writer.WriteValue(SerializeObject(transform.hideFlags));

            writer.WritePropertyName("parent");
            writer.WriteValue(SerializeObject(transform.parent));

            writer.WritePropertyName("scene");
            writer.WriteValue(SerializeObject(gameObject.scene));

            writer.WriteEndObject();
            return (JObject)writer.Token;
        }

        [ContextMenu(nameof(SaveTemp))]
        private void SaveTemp()
        {
            var dat = Save();

            print(dat.Property("worldToLocalMatrix"));

            print(dat.ToSaveableString());
        }
    }
}