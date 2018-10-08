using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ApiNCoreApplication1.Domain;
using ApiNCoreApplication1.Entity.Context;
using ApiNCoreApplication1.Domain.Service;
using Microsoft.Extensions.Logging;
using Serilog;
using ApiNCoreApplication1.Entity;
using System.Threading.Tasks;

namespace ApiNCoreApplication1.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserAsyncController : ControllerBase
    {
        private readonly UserServiceAsync<UserViewModel, User> _userServiceAsync;
        public UserAsyncController(UserServiceAsync<UserViewModel, User> userServiceAsync)
        {
            _userServiceAsync = userServiceAsync;
        }


        //get all
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> GetAll()
        {
            var items = await _userServiceAsync.GetAll();
            return items;
        }

        //get one
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _userServiceAsync.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound();
            }

            return Ok(item);
        }

        //add
        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<IActionResult> Create([FromBody] UserViewModel user)
        {
            if (user == null)
                return BadRequest();

            var id = await _userServiceAsync.Add(user);
            return Created($"api/User/{id}", id);  //HTTP201 Resource created
        }

        //update
        [Authorize(Roles = "Administrator")]
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserViewModel user)
        {
            if (user == null || user.Id != id)
                return BadRequest();

            if (await _userServiceAsync.Update(user))
                return Accepted(user);
            else
                return StatusCode(304);     //Not Modified
        }

        //delete -- it will check ClaimTypes.Role
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _userServiceAsync.Remove(id))
                return NoContent();          //204
            else
                return NotFound();           //404
        }

    }
}


