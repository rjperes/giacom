namespace CDRApi.Model
{
    public class CallDto
    {
        public string? Caller_Id { get; set; }
        public string? Recipient { get; set; }
        public string? Call_Date { get; set; }
        public string? End_Time { get; set; }
        public int Duration { get; set; }
        public decimal Cost { get; set; }
        public string? Reference { get; set; }
        public string? Currency { get; set; }
        public int Type { get; set; }
    }
}
