using Newtonsoft.Json.Linq;

namespace Saveable
{
    public interface ISaveable
    {
        public void Load(JObject data);
        public JObject Save();
    } 
}