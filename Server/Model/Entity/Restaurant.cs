using SqlSugar;

namespace Model.Entity
{
    [SugarTable("restaurant")]
    public class Restaurant
    {
        public int? Id { get; set; }
        public string? shopUrl { get; set; }
        public string? shopName { get; set; }
        public string? shopId { get; set; }
        public string? shopPower { get; set; }
        public string? mainRegionName { get; set; }
        public string? mainCategoryName { get; set; }
        public string? tasteScore { get; set; }
        public string? environmentScore { get; set; }
        public string? serviceScore { get; set; }
        public string? avgPrice { get; set; }
        public string? shopAddress { get; set; }
        public string? defaultPic { get; set; }
        public string? city { get; set; }
    }
}
