using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Data;
using DatingApp.Api.DTOS;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _iAuthRepo;
        private readonly IConfiguration _iConfig;

        public AuthController(IAuthRepository iAuthRepo, IConfiguration iConfig)
        {
            _iConfig = iConfig;
            _iAuthRepo = iAuthRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            userForLoginDto.Username = userForLoginDto.Username.ToLower();
            var userFromRepo = await _iAuthRepo.Login(userForLoginDto.Username, userForLoginDto.Password);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iConfig.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var createdToken = jwtTokenHandler.CreateToken(tokenDescriptor);

            return Ok( new {
                token = jwtTokenHandler.WriteToken(createdToken)
            });
        }
    }
}