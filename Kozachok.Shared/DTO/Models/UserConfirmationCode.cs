using Kozachok.Shared.DTO.Common;
using Kozachok.Utils.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models
{
    public class UserConfirmationCode : Entity
    {
        // -> Empty contructor for EF
        public UserConfirmationCode()
        {

        }

        public UserConfirmationCode(Guid userId, string confirmationCode)
        {
            UserId = userId;
            ConfirmationCode = confirmationCode;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual Guid UserId { get; private set; }
        public virtual string ConfirmationCode { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
