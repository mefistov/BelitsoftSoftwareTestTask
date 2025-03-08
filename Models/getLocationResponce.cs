namespace BelitsoftSoftwareTestTask.Models
{
    public class GetLocationResponce<T>
    {
        public bool IsSuccessful { get; set; }
        public string Status { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public GetLocationResponce(bool isSuccessful, string status, T data, string errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            Status = status;
            Data = data;
            ErrorMessage = errorMessage;
        }
    }
}
