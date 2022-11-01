namespace SerialApi.FormModels
{
    public class AddSerialForm
    {
        public string Name { get; set; } = null!;
        public int ShikiId { get; set; }
        public string Franchise { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? Poster { get; set; }
        public int? Age { get; set; }
        public bool? Strict { get; set; }
        public int? Lenght { get; set; }
        public string? Kind { get; set; }
    }
}
