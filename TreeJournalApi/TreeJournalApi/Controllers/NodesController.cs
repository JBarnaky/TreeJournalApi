using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public NodesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/nodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Node>>> GetNodes()
        {
            return await _context.Nodes.ToListAsync();
        }

        // GET: api/nodes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Node>> GetNode(int id)
        {
            var node = await _context.Nodes.FindAsync(id);
            if (node == null)
            {
                return NotFound();
            }
            return node;
        }

        // POST: api/nodes
        [HttpPost]
        public async Task<ActionResult<Node>> CreateNode(Node node)
        {
            if (string.IsNullOrEmpty(node.Name))
            {
                throw new SecureException("Node name is required.");
            }

            _context.Nodes.Add(node);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNode), new { id = node.Id }, node);
        }

        // PUT: api/nodes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNode(int id, Node node)
        {
            if (id != node.Id)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(node.Name))
            {
                throw new SecureException("Node name is required.");
            }

            _context.Entry(node).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Rethrow to be caught by middleware
                }
            }

            return NoContent();
        }

        // DELETE: api/nodes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _context.Nodes.FindAsync(id);
            if (node == null)
            {
                return NotFound();
            }

            // Check if the node has children
            if (_context.Nodes.Any(n => n.ParentId == id))
            {
                throw new SecureException("You have to delete all children nodes first.");
            }

            _context.Nodes.Remove(node);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NodeExists(int id)
        {
            return _context.Nodes.Any(e => e.Id == id);
        }
    }
}
