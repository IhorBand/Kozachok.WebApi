using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Shared.DTO.Models
{
    public class UserForgetPasswordCode : Entity
    {
        // -> Empty contructor for EF
        public UserForgetPasswordCode()
        {

        }

        public UserForgetPasswordCode(Guid userId, string forgetPasswordCode)
        {
            UserId = userId;
            ForgetPasswordCode = forgetPasswordCode;
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual Guid UserId { get; private set; }
        public virtual string ForgetPasswordCode { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }
    }
}
