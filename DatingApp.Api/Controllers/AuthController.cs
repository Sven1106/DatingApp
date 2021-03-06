using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOS;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository iAuthRepo;
        private readonly IConfiguration iConfig;
        private readonly IMapper mapper;
        public AuthController(IAuthRepository iAuthRepo, IConfiguration iConfig, IMapper mapper) {
            this.mapper = mapper;
            this.iConfig = iConfig;
            this.iAuthRepo = iAuthRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO) {
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            if (await iAuthRepo.UserExists(userForRegisterDTO.Username)) {
                return BadRequest("Username already exists");
            }

            User userToCreate = new User {
                Username = userForRegisterDTO.Username
            };

            User createdUser = await iAuthRepo.Register(userToCreate, userForRegisterDTO.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO) {
            userForLoginDTO.Username = userForLoginDTO.Username.ToLower();
            var userFromRepo = await iAuthRepo.Login(userForLoginDTO.Username, userForLoginDTO.Password);
            if (userFromRepo == null) {
                return Unauthorized();
            }

            var claims = new [] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iConfig.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var createdToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var mainPhoto = mapper.Map<UserForListDTO>(userFromRepo).PhotoUrl;
            return Ok(new {
                token = jwtTokenHandler.WriteToken(createdToken),
                    mainPhoto
            });
        }
    }
}