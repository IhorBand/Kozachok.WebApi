using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Enums;
using Kozachok.Utils.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class UserConfirmationCode : Entity
    {
        // -> Empty contructor for EF
        public UserConfirmationCode()
        {

        }

        public UserConfirmationCode(Guid userId, string confirmationCode, ConfirmationCodeType codeType, byte numberOfAttempt, DateTime nextAttemptDate)
        {
            UserId = userId;
            ConfirmationCode = confirmationCode;
            CreatedDateUTC = DateTime.UtcNow;
            CodeType = codeType;
            NumberOfAttempt = numberOfAttempt;
            NextAttemptDate = nextAttemptDate;
        }

        public virtual Guid UserId { get; private set; }
        public virtual string ConfirmationCode { get; private set; }
        public virtual ConfirmationCodeType CodeType { get; private set; }
        public virtual string AdditionalData { get; private set; }
        public virtual byte NumberOfAttempt { get; private set; }
        public virtual DateTime NextAttemptDate { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }

        public void SetAdditionalData(string data) => AdditionalData = data;
    }
}
