using System.Threading.Tasks;
using DatingApp.Api.Data;
using DatingApp.Api.DTOS;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _iAuthRepo;

        public AuthController(IAuthRepository iAuthRepo)
        {
            _iAuthRepo = iAuthRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            if (await _iAuthRepo.UserExists(userForRegisterDTO.Username))
            {
                return BadRequest("Username already exists");
            }

            User userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            User createdUser = await _iAuthRepo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(201);
        }

    }
}