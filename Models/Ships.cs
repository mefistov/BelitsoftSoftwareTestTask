namespace BelitsoftSoftwareTestTask.Models
{
    public class Ships<T>
    {
        public string ShipId { get; set; }
        public string SeoName { get; set; }
        public string Sitle { get; set; }
        public int Length { get; set; }
        public int Id { get; set; }
        public required List<T> Ship { get; set; }

        public Ships(String shipId, String seoName, string sitle, int length, int id, List<T> ship)
        {
            ShipId = shipId;
            SeoName = seoName;
            Sitle = sitle;
            Length = length;
            Id = id;
            Ship = ship;
        }
    }
    
}