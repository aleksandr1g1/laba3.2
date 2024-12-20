using laba3test.Domain.Interfaces;
using Npgsql;
using Microsoft.Extensions.Configuration;
using laba3test.Domain.Entities;

namespace laba3test.Infrastrucutre
{
    public class UserRepository : RepositoryBase<User>, IRepository<User>
    {
        public UserRepository(IConfiguration configuration) : base(configuration) { }

        public override async Task DeleteAsync(Guid id) =>
            await ExecuteSqlAsync($"DELETE FROM public.users WHERE user_id='{id}'");

        public override async Task<IEnumerable<User>> GetAllAsync() =>
            await ExecuteSqlReaderAsync("SELECT user_id, username, email, favorite_articles, user_comments FROM public.users");

        public override async Task<User> GetByIdAsync(Guid id) =>
            (await ExecuteSqlReaderAsync($"SELECT user_id, username, email, favorite_articles, user_comments FROM public.users WHERE user_id='{id}'")).SingleOrDefault();

        public override async Task<Guid> InsertAsync(User entity)
        {
            var newId = Guid.NewGuid();
            var favArticles = string.Join(",", entity.favoriteArticles ?? Array.Empty<Guid>());
            var userComments = string.Join(",", entity.userComments ?? Array.Empty<Guid>());

            await this.ExecuteSqlAsync(
                $"INSERT INTO public.users (user_id, username, email, favorite_articles, user_comments) 
                VALUES ('{newId}', '{entity.username}', '{entity.email}', '{{{favArticles}}}', '{{{userComments}}}')");

            return newId;
        }

        public override async Task UpdateAsync(Guid id, User entity)
        {
            var favArticles = string.Join(",", entity.favoriteArticles ?? Array.Empty<Guid>());
            var userComments = string.Join(",", entity.userComments ?? Array.Empty<Guid>());

            await this.ExecuteSqlAsync(
                $"UPDATE public.users SET username='{entity.username}', email='{entity.email}',
                favorite_articles='{{{favArticles}}}', user_comments='{{{userComments}}}' 
                WHERE user_id='{id}'");
        }

        protected override User GetEntityFromReader(NpgsqlDataReader reader)
        {
            return new User
            {
                user_id = Guid.Parse(reader["user_id"].ToString()),
                username = reader["username"].ToString(),
                email = reader["email"].ToString(),
                favoriteArticles = reader["favorite_articles"] as Guid[] ?? Array.Empty<Guid>(),
                userComments = reader["user_comments"] as Guid[] ?? Array.Empty<Guid>()
            };
        }
    }
}
