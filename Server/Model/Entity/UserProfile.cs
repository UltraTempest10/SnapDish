using SqlSugar;

namespace Model.Entity
{
    [SugarTable("user_profile")]
    public class UserProfile
    {
        public int? UserId { get; set; }
        public string? Nickname { get; set; }
        public string? Location { get; set; }
        public string? Introduction { get; set; }
    }
}
