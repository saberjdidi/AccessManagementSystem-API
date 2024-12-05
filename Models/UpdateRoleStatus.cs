namespace AccessManagementSystem_API.Models
{
    public class UpdateRoleStatus
    {
        public string email { get; set; }
        //public string role { get; set; }
        public List<string>? Roles { get; set; }
        public bool status { get; set; }
    }
}
