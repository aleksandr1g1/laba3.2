using laba3test.Domain.Interfaces;
using Npgsql;
using Microsoft.Extensions.Configuration;
using laba3test.Domain.Entities;

namespace laba3test.Infrastrucutre
{
    public class ArticleRepository : RepositoryBase<Article>, IRepository<Article>
    {
        public ArticleRepository(IConfiguration configuration) : base(configuration) { }

        public override async Task DeleteAsync(Guid id) =>
            await ExecuteSqlAsync($"DELETE FROM public.articles WHERE article_id='{id}'");

        public override async Task<IEnumerable<Article>> GetAllAsync() =>
            await ExecuteSqlReaderAsync("SELECT article_id, genre, title, artist, content FROM public.articles");

        public override async Task<Article> GetByIdAsync(Guid id) =>
            (await ExecuteSqlReaderAsync($"SELECT article_id, genre, title, artist, content FROM public.articles WHERE article_id='{id}'")).SingleOrDefault();

        public override async Task<Guid> InsertAsync(Article entity)
        {
            var newId = Guid.NewGuid();
            await ExecuteSqlAsync(
                $"INSERT INTO public.articles (article_id, genre, title, artist, content) " +
                $"VALUES ('{newId}', '{entity.genre}', '{entity.title}', '{entity.artist}', '{entity.content}')");

            return newId;
        }

        public override async Task UpdateAsync(Guid id, Article entity)
        {
            await ExecuteSqlAsync(
                $"UPDATE public.articles SET genre='{entity.genre}', title='{entity.title}', artist='{entity.artist}', content='{entity.content}' " +
                $"WHERE article_id='{id}'");
        }

        protected override Article GetEntityFromReader(NpgsqlDataReader reader)
        {
            return new Article
            {
                article_id = Guid.Parse(reader["article_id"].ToString()),
                genre = reader["genre"].ToString(),
                title = reader["title"].ToString(),
                artist = reader["artist"].ToString(),
                content = reader["content"].ToString()
            };
        }
    }

}
