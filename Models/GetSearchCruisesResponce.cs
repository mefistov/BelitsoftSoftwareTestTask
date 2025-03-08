namespace BelitsoftSoftwareTestTask.Models
{
    public class GetSearchCruisesResponce<T>
    {
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public List<T> List { get; set; }
    
    public GetSearchCruisesResponce(int totalPages, int totalResults, List<T> ships)
        {
            TotalPages = totalPages;
            TotalResults = totalResults;
            List = ships;
        }
    }
}