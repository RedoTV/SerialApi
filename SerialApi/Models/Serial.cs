using System.ComponentModel.DataAnnotations;

namespace SerialApi.Models
{
    public class Serial
    {
        public Serial(string name, string description, string pathToPoster,string shikiId)
        {
            Name = name;
            Description = description;
            PathToPoster = pathToPoster;
            ShikiId = shikiId;
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string PathToPoster { get; set; } = null!;
        [Required]
        public string ShikiId { get; set; } = null!;
    }
}
