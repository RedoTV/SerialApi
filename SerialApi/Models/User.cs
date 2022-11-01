using System.ComponentModel.DataAnnotations;

namespace SerialApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
