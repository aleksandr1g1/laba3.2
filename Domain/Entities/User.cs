namespace laba3test.Domain.Entities
{
    public class User
    {
        public Guid? user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public Guid[] favoriteArticles { get; set; }
        public Guid[] userComments { get; set; }

    }
}
