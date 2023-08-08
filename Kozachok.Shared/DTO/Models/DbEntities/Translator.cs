using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class Translator : Entity
    {
        // -> Empty contructor for EF
        public Translator()
        {
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; set; }
        public virtual string TranslatorId { get; set; }
        public virtual TranslatorLanguage LanguageId { get; set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
