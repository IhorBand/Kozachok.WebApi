namespace Kozachok.WebApi.Models.Common
{
    public class ErrorMessage
    {
        public string? Message { get; set; }
        public DateTime DateTime { get; private set; }
    }
}
