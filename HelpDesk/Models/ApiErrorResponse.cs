namespace HelpDesk.Models
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
