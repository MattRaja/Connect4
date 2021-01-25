using Newtonsoft.Json;

namespace GameEngine
{
    public static class GameConfigHandler
    {
        private const string FileName = "gamesettings.json";
        
        public static void SaveConfig(GameSettings settings, string fileName = FileName)
        {
            using (var writer = System.IO.File.CreateText(fileName))
            {
                writer.Write(JsonConvert.SerializeObject(settings,Formatting.None, new JsonSerializerSettings
                {
                    TypeNameHandling =TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                }));
            }
        }

        public static GameSettings LoadConfig(string fileName = FileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                var jsonString = System.IO.File.ReadAllText(fileName);
//                var res = JsonConvert.DeserializeObject(jsonString,
//                    new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects});
                var res = JsonConvert.DeserializeObject<GameSettings>(jsonString);
                return new GameSettings()
                {
                    GameName = res.GameName,
                    BoardHeight = res.BoardHeight,
                    BoardWidth = res.BoardWidth,
                };
            }
            
            return new GameSettings();
        }
    }
}