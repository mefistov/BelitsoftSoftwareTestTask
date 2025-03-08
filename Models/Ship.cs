namespace BelitsoftSoftwareTestTask.Models
{
    public class Ship
    {
        public string ShipId { get; set; }
        public string Name { get; set; }
        public int Crew { get; set; }

        public Ship(string shipId, string name, int crew)
        {
            ShipId = shipId;
            Name = name;
            Crew = crew;
        }
    }
}