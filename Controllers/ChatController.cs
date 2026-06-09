using IELTSWritingAI.Data;
using IELTSWritingAI.Models;
using IELTSWritingAI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IELTSWritingAI.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEssayScoringService _essayScoringService;

    public ChatController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEssayScoringService essayScoringService)
    {
        _db = db;
        _userManager = userManager;
        _essayScoringService = essayScoringService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int submissionId, string message, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(message))
        {
            return RedirectToAction("Result", "Writing", new { id = submissionId });
        }

        var submission = await _db.Submissions
            .Include(x => x.Topic)
            .FirstOrDefaultAsync(x => x.Id == submissionId && x.UserId == userId, cancellationToken);

        if (submission is null)
        {
            return NotFound();
        }

        var userMessage = new ChatMessage
        {
            SubmissionId = submissionId,
            UserId = userId,
            Role = "user",
            Message = message.Trim()
        };

        _db.ChatMessages.Add(userMessage);
        await _db.SaveChangesAsync(cancellationToken);

        var recentMessages = await _db.ChatMessages
            .Where(x => x.SubmissionId == submissionId && x.UserId == userId)
            .OrderByDescending(x => x.CreatedDate)
            .Take(10)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var assistantReply = await _essayScoringService.TutorChatAsync(submission, recentMessages, userMessage.Message, cancellationToken);
        _db.ChatMessages.Add(new ChatMessage
        {
            SubmissionId = submissionId,
            UserId = userId,
            Role = "assistant",
            Message = assistantReply
        });
        await _db.SaveChangesAsync(cancellationToken);

        return RedirectToAction("Result", "Writing", new { id = submissionId });
    }
}
