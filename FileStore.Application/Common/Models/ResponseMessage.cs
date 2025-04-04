namespace FileStore.Application.Common.Models
{
    public class ResponseMessage<T>
    {
        public string Message { get; set; }
        public T Payload { get; set; }
        public bool Success { get; set; }
    }
}
