namespace events.Dto
{
    public class MovieDto
    {
        public Guid movie_id { get; set; }

        public string title { get; set; }

        public string action { get; set; }

        public Guid user_id { get; set; }
    }
}
