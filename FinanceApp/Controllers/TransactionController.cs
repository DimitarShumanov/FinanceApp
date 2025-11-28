using FinanceApp.Models;
using FinanceApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace FinanceApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return View(transactions);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(transactionDto);
            }

            var category = await _context.Categories.FindAsync(transactionDto.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CatgeoryId", "Invalid category!");
                ViewBag.Category = _context.Categories.ToList();
                return View(transactionDto);
            }

            var transaction = new Transaction
            {
                UserId = User.Identity.Name,
                CategoryId = transactionDto.CategoryId,
                Date = transactionDto.Date == default ? DateTime.Now : transactionDto.Date,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description,
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId != User.Identity.Name)
                return NotFound();

            var dto = new TransactionDto
            {
                CategoryId = transaction.CategoryId,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Description = transaction.Description
            };

            ViewBag.Categories = _context.Categories.ToList();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TransactionDto transactionDto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId != User.Identity.Name) 
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(transactionDto);
            }

            var category = await _context.Categories.FindAsync(transactionDto.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category!");
                ViewBag.Categories = _context.Categories.ToList();
                return View(transactionDto);
            }

            transaction.CategoryId = transactionDto.CategoryId;
            transaction.Date = transactionDto.Date == default ? DateTime.Now : transactionDto.Date;
            transaction.Amount = transactionDto.Amount;
            transaction.Description = transactionDto.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == User.Identity.Name);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
