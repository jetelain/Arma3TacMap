﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Entities
{
    public class MessageLineTemplate
    {
        [JsonIgnore]
        public int MessageLineTemplateID { get; set; }

        [JsonIgnore]
        public int MessageTemplateID { get; set; }

        [JsonIgnore]
        public MessageTemplate? MessageTemplate { get; set; }

        [Display(Name = "Number")]
        [JsonIgnore]
        public int SortNumber { get; set; }

        [Display(Name = "Prefix")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        public List<MessageFieldTemplate>? Fields { get; set; }
    }
}