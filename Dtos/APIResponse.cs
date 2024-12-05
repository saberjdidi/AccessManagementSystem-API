namespace AccessManagementSystem_API.Dtos
{
    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Result { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
