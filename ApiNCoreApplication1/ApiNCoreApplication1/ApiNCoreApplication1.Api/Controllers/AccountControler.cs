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
    public class AccountController : ControllerBase
    {
        private readonly AccountService<AccountViewModel, Account> _accountService;
        public AccountController(AccountService<AccountViewModel, Account> accountService)
        {
            _accountService = accountService;
        }

        //get all
        [Authorize]
        [HttpGet]
        public IEnumerable<AccountViewModel> GetAll()
        {
            //Log.Information("Log: Log.Information");
            //Log.Warning("Log: Log.Warning");
            //Log.Error("Log: Log.Error");
            //Log.Fatal("Log: Log.Fatal");
            var test = _accountService.DoNothing();
            var items = _accountService.GetAll();
            return items;
        }

        //get one
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _accountService.GetOne(id);
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
        public IActionResult Create([FromBody] AccountViewModel account)
        {
            if (account == null)
                return BadRequest();

            var id = _accountService.Add(account);
            return Created($"api/Account/{id}", id);  //HTTP201 Resource created
        }

        //update
        [Authorize(Roles = "Administrator")]
        //[HttpPost("{id}")]
        [HttpPost("{id}")]
        public IActionResult Update(int id, [FromBody] AccountViewModel account)
        {
            if (account == null || account.Id != id)
                return BadRequest();

            if (_accountService.Update(account))
                return Accepted(account);
            else
                return StatusCode(304);     //Not Modified
        }

        //delete -- it will check ClaimTypes.Role
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_accountService.Remove(id))
                return NoContent();          //204
            else
                return NotFound();           //404
        }
    }
}


