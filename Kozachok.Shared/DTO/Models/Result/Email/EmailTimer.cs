using System;

namespace Kozachok.Shared.DTO.Models.Result.Email
{
    public class EmailTimer
    {
        public DateTime NextAttemptDate { get; set; }
        public byte AttemtNumber { get; set; }
    }
}
