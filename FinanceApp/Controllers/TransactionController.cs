using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace FinanceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Transaction>>> GetTransactions()
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t->t.Id == id);

            if (transaction == null)
                return NotFound("Transaction is not found!");

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            if (transaction.Amount <= 0)
                return BadRequest("Amount must be greater than zero!");

            if (transaction.CategoryId <= 0)
                return BadRequest("Category Id is required!");

            var category = await _context.Categories.FindAsync(transaction.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID");

            if (transaction.Date == default)
                transaction.Date = DateTime.Now;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction),
                new { id = transaction.Id }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
                return BadRequest("ID mismatch!");

            if (transaction.Amount <= 0)
                return BadRequest("Invalid amount!");

            var category = await _context.Categories.FindAsync(transaction.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID!");

            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound("Transaction does not exist!");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
