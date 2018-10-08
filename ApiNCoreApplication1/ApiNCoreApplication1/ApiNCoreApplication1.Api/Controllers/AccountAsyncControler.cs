using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ApiNCoreApplication1.Domain;
using ApiNCoreApplication1.Entity.Context;
using ApiNCoreApplication1.Domain.Service;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using ApiNCoreApplication1.Entity;

namespace ApiNCoreApplication1.Api.Controllers
{
    [Route("api/[controller]")]
    public class AccountAsyncController : ControllerBase
    {
        private readonly AccountServiceAsync<AccountViewModel, Account> _accountServiceAsync;
        public AccountAsyncController(AccountServiceAsync<AccountViewModel, Account> accountServiceAsync)
        {
            _accountServiceAsync = accountServiceAsync;
        }


        //get all
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _accountServiceAsync.GetAll();
            return Ok(items);
        }

        //get one
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _accountServiceAsync.GetOne(id);
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
        public async Task<IActionResult> Create([FromBody] AccountViewModel account)
        {
            if (account == null)
                return BadRequest();

            var id = await _accountServiceAsync.Add(account);
            return Created($"api/Account/{id}", id);  //HTTP201 Resource created
        }

        //update
        [Authorize(Roles = "Administrator")]
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccountViewModel account)
        {
            if (account == null || account.Id != id)
                return BadRequest();
            if (await _accountServiceAsync.Update(account))
                return Accepted(account);
            else
                return StatusCode(304);
        }


        //delete -- it will check ClaimTypes.Role
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _accountServiceAsync.Remove(id))
                return NoContent();   	     //204
            else
                return NotFound();           //404
        }
    }
}


