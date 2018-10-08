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

namespace ApiNCoreApplication1.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService<UserViewModel, User> _userService;
        public UserController(UserService<UserViewModel, User> userService)
        {
            _userService = userService;
        }

        //get all
        [Authorize]
        [HttpGet]
        public IEnumerable<UserViewModel> GetAll()
        {
            var test = _userService.DoNothing();
            var items = _userService.GetAll();
            return items;
        }

        //get one
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _userService.GetOne(id);
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
        public IActionResult Create([FromBody] UserViewModel user)
        {
            if (user == null)
                return BadRequest();

            var id = _userService.Add(user);
            return Created($"api/User/{id}", id);  //HTTP201 Resource created
        }

        //update
        [Authorize(Roles = "Administrator")]
        [HttpPost("{id}")]
        public IActionResult Update(int id, [FromBody] UserViewModel user)
        {
            if (user == null || user.Id != id)
                return BadRequest();

            if (_userService.Update(user))
                return Accepted(user);
            else
                return StatusCode(304);     //Not Modified
        }

        //delete -- it will check ClaimTypes.Role
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_userService.Remove(id))
                return NoContent();          //204
            else
                return NotFound();           //404
        }

    }
}


