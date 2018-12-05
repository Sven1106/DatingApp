using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Api.Models;
using DatingApp.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _db;

        // GET api/values

        public ValuesController(DataContext db)
        {
            _db = db;
        }
        /* When sync Api calls are being made, it is made on a user level. That means a returning a list of 1mil values wont block other users from making the same call. */


        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues() //IActionResult can return HttpResponses to the client. 
        {
            List<Value> values = await _db.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult>  GetValue(int id)
        {
            Value value = await _db.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post(string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
