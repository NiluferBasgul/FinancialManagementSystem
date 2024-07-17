using System.ComponentModel.DataAnnotations;

namespace FinancialManagementSystem.Core.Models
{
    public class UserUpdateModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
