namespace BelitsoftSoftwareTestTask.Models
{
    public class GetSearchCruisesResponce<T>
    {
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public T List { get; set; }
        public string Price { get; set; }
        public T Filters { get; set; }
        public string Status { get; set; }

        public GetSearchCruisesResponce(int totalPages, int totalResults, T list, string price, T filters)
        {
            TotalPages = totalPages;
            TotalResults = totalResults;
            List = list;
            Price = price;
            Filters = filters;
            Status = string.Empty;
        }
    }
}