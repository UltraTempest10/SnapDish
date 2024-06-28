namespace Model.Entity
{
    public class City(string name, double latitude, double longitude)
    {
        public string Name { get; set; } = name;
        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;
    }
}
