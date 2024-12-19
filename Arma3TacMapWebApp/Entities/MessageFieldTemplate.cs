using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Number")]
        [JsonIgnore]
        public int SortNumber { get; set; }

        [Display(Name = "Prefix")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [Display(Name = "Input type")]
        [JsonConverter(typeof(JsonStringEnumConverter<MessageFieldType>))]
        public MessageFieldType Type { get; set; }
    }
}