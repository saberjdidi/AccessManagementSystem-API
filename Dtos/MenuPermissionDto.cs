namespace AccessManagementSystem_API.Dtos
{
    public class MenuPermissionDto
    {
        public string code { get; set; }
        public string Name { get; set; }
        public bool Haveview { get; set; }
        public bool Haveadd { get; set; }
        public bool Haveedit { get; set; }
        public bool Havedelete { get; set; }
    }
}
