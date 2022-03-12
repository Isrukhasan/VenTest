using System.ComponentModel.DataAnnotations;

namespace Venturus.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
