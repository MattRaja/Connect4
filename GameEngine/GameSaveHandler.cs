using System;
using System.IO;
using Newtonsoft.Json;

namespace GameEngine
{
    public static class GameSaveHandler
    {
        public static string BoardSerializer(GameState settings)
        {
            return JsonConvert.SerializeObject(settings, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });

        }

        public static GameState BoardDeserializer(string serializedString)
        {
            var res = JsonConvert.DeserializeObject<GameState>(serializedString);
            return new GameState()
            {
                Board = res.Board,
                GameName = res.GameName,
                Turn = res.Turn
            };
        }
    }
}