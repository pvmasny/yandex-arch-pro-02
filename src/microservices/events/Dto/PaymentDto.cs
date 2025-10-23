namespace events.Dto
{
    public class PaymentDto
    {
        public Guid payment_id { get; set; }

        public Guid user_id { get; set; }

        decimal amount { get; set; }

        public string status { get; set; }

        public string timestamp { get; set; }

        public string method_type { get; set; }
    }
}
