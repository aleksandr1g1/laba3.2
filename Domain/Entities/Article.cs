namespace laba3test.Domain.Entities
{
    public class Article
    {
        public Guid? article_id { get; set; }
        public string genre { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string content { get; set; }

    }
}
