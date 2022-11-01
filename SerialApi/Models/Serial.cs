using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SerialApi.Models
{
    public class Serial
    {
        public Serial(string name, string description, int shikiId , string franchise , string poster , int? lenght, int? age ,bool? strict , string? kind)
        {
            Name = name;
            Franchise = franchise;
            Description = description;
            Poster = poster;
            ShikiId = shikiId;
            Lenght = lenght;
            Age = age;
            Strict = strict;
            Kind = kind;
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public string Franchise { get; set; } = null!;
        public string Description { get; set; } = null!;

        [Required]
        public int ShikiId { get; set; }
        public string Poster { get; set; } = null!;
        public int? Lenght { get; set; }

        [DefaultValue(0)]
        public int? Exist { get; set; }

        [DefaultValue(0d)]
        public decimal? Rating { get; set; }
        public string? Kind { get; set; }
        
        public int? Age { get; set; }
        public bool? Strict { get; set; }
    }
}
