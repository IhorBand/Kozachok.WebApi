using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DomainEntities
{
    public class TranslatorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TranslatorId { get; set; }
        public TranslatorLanguage LanguageId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
