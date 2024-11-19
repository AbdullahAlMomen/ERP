namespace ERP.Models
{
    public class CommonResponse
    {
        public int Code { get; set; }
        public Object? Data { get; set; }
        public string? Status { get; set; }
        public List<string> Message { get; set; }=new List<string>();
    }
}
