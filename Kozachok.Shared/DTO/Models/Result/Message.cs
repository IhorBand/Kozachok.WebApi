using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.Result
{
    public class Message<T>
    {
        public bool IsSuccess { get; set; }
        public string Text { get; set; }
        public T Result { get; set; }
    }
}
