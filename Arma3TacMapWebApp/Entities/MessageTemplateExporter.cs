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
                href,
                messageTemplate.Lines!.Select(l => new object?[]
                {
                    l.Title,
                    l.Description,
                    l.Fields!.Select(f => new object?[]
                    {
                        f.Title,
                        f.Description,
                        (int)f.Type
                    }).ToArray()
                }).ToArray()
            };
        }

        public static MessageTemplateJson ToJson(MessageTemplate messageTemplate, string uid, string href)
        {
            return new MessageTemplateJson(messageTemplate, uid, href);
        }
    }
}
