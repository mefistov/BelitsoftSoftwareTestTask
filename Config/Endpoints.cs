namespace BelitsoftSoftwareTestTask.Config
{
    public static class Endpoints
    {
        public static string GetCruisesLocation { get; } = "/api/v1/cruises/getLocation";
        public static string SearchCruises { get; } = "/api/v1/cruises/searchCruises";
    }
}