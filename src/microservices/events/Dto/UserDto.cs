namespace events.Dto
{
    public class UserDto
    {
        public Guid user_id { get; set; }

        public string username { get; set; }

        public string action { get; set; }

        public string timestamp { get; set; }
    }
}
