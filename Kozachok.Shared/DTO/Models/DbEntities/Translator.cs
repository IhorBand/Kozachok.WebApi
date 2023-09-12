using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class Translator : Entity
    {
        public Translator()
        {
            CreatedDateUtc = DateTime.UtcNow;
        }

        public virtual string Name { get; set; }
        public virtual string TranslatorId { get; set; }
        public virtual TranslatorLanguage LanguageId { get; set; }
        public virtual DateTime CreatedDateUtc { get; private set; }
    }
}
