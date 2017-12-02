namespace azure_ad_b2b_shared
{
    public class ServiceResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Value { get; set; }
        public ServiceResult() { }

        public ServiceResult(T value)
        {
            Value = value;
            Success = true;
        }
    }
}