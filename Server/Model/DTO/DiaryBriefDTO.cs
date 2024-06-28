
namespace Model.DTO
{
    public class DiaryBriefDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? Image { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
