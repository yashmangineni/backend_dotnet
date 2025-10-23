using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RatingsController(AppDbContext db)
        {
            _db = db;
        }

        // POST: api/ratings
        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] RatingDto ratingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data provided" });

            var rating = new Rating
            {
                Score = ratingDto.Score,
                Comment = ratingDto.Comment,
                Date = DateTime.UtcNow
            };

            _db.Ratings.Add(rating);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Rating submitted successfully" });
        }

        // GET: api/ratings
        [HttpGet]
        public async Task<IActionResult> GetRatings()
        {
            var ratings = await _db.Ratings
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            // Calculate average rating
            var averageRating = ratings.Count > 0 ? ratings.Average(r => r.Score) : 0;
            var totalRatings = ratings.Count;

            return Ok(new { 
                success = true, 
                data = ratings,
                averageRating = Math.Round(averageRating, 1),
                totalRatings = totalRatings
            });
        }

        // GET: api/ratings/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetRatingStats()
        {
            var ratings = await _db.Ratings.ToListAsync();

            if (ratings.Count == 0)
            {
                return Ok(new
                {
                    success = true,
                    averageRating = 0,
                    totalRatings = 0,
                    distribution = new Dictionary<int, int>
                    {
                        { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
                    }
                });
            }

            var averageRating = ratings.Average(r => r.Score);
            var totalRatings = ratings.Count;

            // Create distribution (count of each rating)
            var distribution = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                distribution[i] = ratings.Count(r => r.Score == i);
            }

            return Ok(new
            {
                success = true,
                averageRating = Math.Round(averageRating, 1),
                totalRatings = totalRatings,
                distribution = distribution
            });
        }
    }

    // DTO for rating creation
    public class RatingDto
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}