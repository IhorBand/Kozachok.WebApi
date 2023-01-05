namespace Kozachok.Shared.DTO.Models.Result.Common
{
    public class Message<T>
    {
        public bool IsSuccess { get; set; }
        public string Text { get; set; }
        public T Result { get; set; }
    }
}
