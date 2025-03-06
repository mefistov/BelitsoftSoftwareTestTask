namespace BelitsoftSoftwareTestTask.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
