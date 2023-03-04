namespace Entity
{
    public class BaseResult<T> where T : class
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Exception { get; set; }
    }
}
