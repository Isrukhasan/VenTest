using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Venturus.ViewModel
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            UsersList = new List<string>();

        }

        public string Id { get; set; }
        [Required(ErrorMessage = "Role needed")]
        public string RoleName { get; set; }
        public List<string> UsersList { get; set;}
    }
}
