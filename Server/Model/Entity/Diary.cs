using SqlSugar;

namespace Model.Entity
{
    [SugarTable("diary")]
    public class Diary
    {
        public int DiaryId { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public int? Rating { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
