using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Enums
{
    public enum ConfirmationCodeType : byte
    {
        Other = 0,
        EmailConfirmation = 1,
        ForgotPassword = 2,
        ChangeEmail = 3
    }
}
