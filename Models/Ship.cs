namespace BelitsoftSoftwareTestTask.Models
{
    public class Ship
    {
        public string shipId { get; set; }
        public string name { get; set; }
        public int crew { get; set; }

        public Ship(string shipId, string name, int crew)
        {
            this.shipId = shipId;
            this.name = name;
            this.crew = crew;
        }
    }
}