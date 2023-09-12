using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using System;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class UserConfirmationCode : Entity
    {
        public virtual Guid UserId { get; private set; }
        public virtual string ConfirmationCode { get; private set; }
        public virtual ConfirmationCodeType CodeType { get; private set; }
        public virtual string AdditionalData { get; private set; }
        public virtual byte NumberOfAttempt { get; private set; }
        public virtual DateTime NextAttemptDate { get; private set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public void SetAdditionalData(string data) => AdditionalData = data;

        public static UserConfirmationCode Create(Guid userId, string confirmationCode, ConfirmationCodeType codeType, byte numberOfAttempt, DateTime nextAttemptDate)
        {
            return new UserConfirmationCode
            {
                UserId = userId,
                ConfirmationCode = confirmationCode,
                CreatedDateUtc = DateTime.UtcNow,
                CodeType = codeType,
                NumberOfAttempt = numberOfAttempt,
                NextAttemptDate = nextAttemptDate
            };
        }
    }
}
