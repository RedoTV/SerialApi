namespace SerialApi.FormModels
{
    public class ChangeSerialForm
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Franchise { get; set; }
        public string? Description { get; set; }
        public IFormFile? Poster { get; set; }
        public int? Age { get; set; }
        public bool? Strict { get; set; }
        public int? Lenght { get; set; }
        public string? Kind { get; set; }
    }
}
