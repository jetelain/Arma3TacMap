using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Entities
{
    public class MessageFieldTemplate
    {
        [JsonIgnore]
        public int MessageFieldTemplateID { get; set; }

        [JsonIgnore]
        public int MessageLineTemplateID { get; set; }

        [JsonIgnore]
        public MessageLineTemplate? MessageLineTemplate { get; set; }

        [JsonIgnore]
        public int SortNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<MessageFieldType>))]
        public MessageFieldType Type { get; set; }
    }
}