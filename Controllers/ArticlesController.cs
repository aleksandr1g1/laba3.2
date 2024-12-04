using laba3test.Domain.Entities;
using laba3test.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace laba3test.Controllers
{
    [ApiController]
    [Route("article")]
    public class ArticlesController : ControllerBase
    {
        private readonly ILogger<ArticlesController> logger;
        private readonly IRepository<Article> articleRepository;

        public ArticlesController(ILogger<ArticlesController> logger, IRepository<Article> articleRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> Get()
        {
            logger.LogInformation("Get all articles");
            return Ok(await articleRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> Get(Guid id)
        {
            var article = await articleRepository.GetByIdAsync(id);
            if (article == null) return NotFound($"Статья с ID {id} не найдена.");

            return Ok(article);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] Article article)
        {
            var validationResult = ValidateArticle(article);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (!article.article_id.HasValue)
                article.article_id = Guid.NewGuid();

            return Ok(await articleRepository.InsertAsync(article));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Article article)
        {
            var validationResult = ValidateArticle(article);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (await articleRepository.GetByIdAsync(id) == null)
                return NotFound($"Статья с ID {id} не существует.");

            await articleRepository.UpdateAsync(id, article);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await articleRepository.GetByIdAsync(id) == null)
                return NotFound($"Статья с ID {id} не существует.");

            await articleRepository.DeleteAsync(id);
            return Ok();
        }

        private string ValidateArticle(Article article)
        {
            if (article == null) return "Статья не может быть пустой.";
            if (string.IsNullOrWhiteSpace(article.title)) return "Заголовок статьи обязателен.";
            if (string.IsNullOrWhiteSpace(article.content)) return "Содержимое статьи обязательно.";
            return string.Empty;
        }
    }

}
