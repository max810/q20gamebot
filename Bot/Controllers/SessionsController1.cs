using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bot.Q20GameBot.Models;

namespace Bot.Controllers
{
    [Produces("application/json")]
    [Route("api/SessionsController1")]
    public class SessionsController1 : Controller
    {
        private readonly SessionsContext _context;

        public SessionsController1(SessionsContext context)
        {
            _context = context;
        }

        // GET: api/SessionsController1
        [HttpGet]
        public IEnumerable<Session> GetSessions()
        {
            return _context.Sessions;
        }

        // GET: api/SessionsController1/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _context.Sessions.SingleOrDefaultAsync(m => m.ChatId == id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        // PUT: api/SessionsController1/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSession([FromRoute] long id, [FromBody] Session session)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != session.ChatId)
            {
                return BadRequest();
            }

            _context.Entry(session).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SessionsController1
        [HttpPost]
        public async Task<IActionResult> PostSession([FromBody] Session session)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Sessions.Add(session);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SessionExists(session.ChatId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSession", new { id = session.ChatId }, session);
        }

        // DELETE: api/SessionsController1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _context.Sessions.SingleOrDefaultAsync(m => m.ChatId == id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        private bool SessionExists(long id)
        {
            return _context.Sessions.Any(e => e.ChatId == id);
        }
    }
}