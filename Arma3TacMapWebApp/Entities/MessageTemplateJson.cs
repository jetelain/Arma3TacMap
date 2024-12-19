using System.Collections.Generic;

namespace Arma3TacMapWebApp.Entities
{
    public class MessageTemplateJson
    {
        public string Uid { get; set; }

        public List<MessageLineTemplate> Lines { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public MessageTemplateType Type { get; set; }

        public string Href { get; set; }

        public MessageTemplateJson(MessageTemplate messageTemplate, string uid, string href)
        {
            Uid = uid;
            Lines = messageTemplate.Lines!;
            Title = messageTemplate.Title;
            Description = messageTemplate.Description;
            Type = messageTemplate.Type;
            Href = href;
        }
    }
}
