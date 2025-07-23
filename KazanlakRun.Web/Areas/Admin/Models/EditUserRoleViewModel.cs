namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class EditUserRoleViewModel
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string SelectedRole { get; set; } = default!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}

