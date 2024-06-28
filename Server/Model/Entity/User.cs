using SqlSugar;

namespace Model.Entity
{
    [SugarTable("user")]
    public class User
    {
        public int? UserId { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? Permission { get; set; }

    }
}
