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
    [Route("user")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IRepository<User> userRepository;

        public UsersController(ILogger<UsersController> logger, IRepository<User> userRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            logger.LogInformation("Get all users");
            return Ok(await userRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null) return NotFound($"Пользователь с ID {id} не найден.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (!user.user_id.HasValue)
                user.user_id = Guid.NewGuid();

            return Ok(await userRepository.InsertAsync(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (await userRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await userRepository.UpdateAsync(id, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await userRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await userRepository.DeleteAsync(id);
            return Ok();
        }

        private string ValidateUser(User user)
        {
            if (user == null) return "Пользователь не может быть пустым.";
            if (string.IsNullOrWhiteSpace(user.username)) return "Имя пользователя обязательно.";
            return string.Empty;
        }
    }

}
