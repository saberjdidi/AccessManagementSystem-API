namespace AccessManagementSystem_API.Dtos
{
    public class ResponseDto
    {
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccess { get; set; }
        public object? Result { get; set; }
    }
}
