using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.Result.Email
{
    public class EmailTimer
    {
        public DateTime NextAttemptDate { get; set; }
        public byte AttemtNumber { get; set; }
    }
}
