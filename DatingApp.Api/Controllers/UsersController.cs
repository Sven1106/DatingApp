using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly IDatingRepository datingRepo;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository datingRepo, IMapper mapper) {
            this.datingRepo = datingRepo;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers() {
            var users = await datingRepo.GetUsers();
            var usersToReturn = mapper.Map<IEnumerable<UserForListDTO>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id) {
            var user = await datingRepo.GetUser(id);
            var userToReturn = mapper.Map<UserForDetailsDTO>(user);
            return Ok(userToReturn);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserForUpdateDTO userForUpdateDTO) {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await datingRepo.GetUser(currentUserId);
            mapper.Map(userForUpdateDTO, userFromRepo);

            if (!await datingRepo.SaveAll()) {
                return Conflict();
            }

            return NoContent();
        }
    }
}