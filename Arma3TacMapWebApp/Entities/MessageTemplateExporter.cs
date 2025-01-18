using System.Linq;

namespace Arma3TacMapWebApp.Entities
{
    public static class MessageTemplateExporter
    {
        public static object[] ToArma3(MessageTemplate messageTemplate, string uid, string href)
        {
            return new object[]
            {
                uid,
                (int)messageTemplate.Type,
                messageTemplate.Title,
                ShortTitle(messageTemplate),
                href,
                messageTemplate.Lines!.Select(l => new object?[]
                {
                    l.Title ?? string.Empty,
                    l.Description ?? string.Empty,
                    l.Fields!.Select(f => new object?[]
                    {
                        f.Title ?? string.Empty,
                        f.Description ?? string.Empty,
                        (int)f.Type
                    }).ToArray()
                }).ToArray()
            };
        }

        private static string ShortTitle(MessageTemplate messageTemplate)
        {
            if (messageTemplate.Title.Length > 12)
            {
                if (messageTemplate.Lines != null && messageTemplate.Lines.Count > 0)
                {
                    var firstLine = messageTemplate.Lines[0];
                    if (!string.IsNullOrEmpty(firstLine.Title))
                    {
                        return firstLine.Title;
                    }
                }
                return messageTemplate.Title.Substring(0, 12);
            }
            return messageTemplate.Title;
        }

        public static MessageTemplateJson ToJson(MessageTemplate messageTemplate, string uid, string href)
        {
            return new MessageTemplateJson(messageTemplate, uid, href);
        }
    }
}
