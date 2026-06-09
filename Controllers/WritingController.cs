using IELTSWritingAI.Data;
using IELTSWritingAI.Models;
using IELTSWritingAI.Services;
using IELTSWritingAI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IELTSWritingAI.Controllers;

[Authorize]
public class WritingController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEssayScoringService _essayScoringService;

    public WritingController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEssayScoringService essayScoringService)
    {
        _db = db;
        _userManager = userManager;
        _essayScoringService = essayScoringService;
    }

    public async Task<IActionResult> Index()
    {
        var submissions = await _db.Submissions
            .Include(x => x.Topic)
            .Where(x => x.UserId == _userManager.GetUserId(User))
            .OrderByDescending(x => x.CreatedDate)
            .Take(8)
            .ToListAsync();

        return View(submissions);
    }

    [HttpGet]
    public async Task<IActionResult> Practice(WritingTaskType taskType = WritingTaskType.Task2)
    {
        var topics = await _db.WritingTopics
            .Where(x => x.TaskType == taskType)
            .ToListAsync();

        if (topics.Count == 0)
        {
            return NotFound("No writing topics found.");
        }

        var topic = topics[Random.Shared.Next(topics.Count)];
        return View(new WritingPracticeViewModel
        {
            TopicId = topic.Id,
            TaskType = topic.TaskType,
            TopicText = topic.TopicText
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(WritingPracticeViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.EssayText))
        {
            ModelState.AddModelError(nameof(model.EssayText), "Please write your essay before submitting.");
        }

        var topic = await _db.WritingTopics.FindAsync([model.TopicId], cancellationToken);
        if (topic is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.TopicText = topic.TopicText;
            model.TaskType = topic.TaskType;
            return View("Practice", model);
        }

        var score = await _essayScoringService.ScoreEssayAsync(topic, model.EssayText, cancellationToken);
        var submission = new Submission
        {
            UserId = _userManager.GetUserId(User) ?? string.Empty,
            TopicId = topic.Id,
            EssayText = model.EssayText.Trim(),
            WordCount = CountWords(model.EssayText),
            OverallScore = score.Overall,
            TaskAchievement = score.TaskAchievement,
            CoherenceCohesion = score.CoherenceCohesion,
            LexicalResource = score.LexicalResource,
            GrammarRangeAccuracy = score.GrammarRangeAccuracy,
            Feedback = score.Feedback
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Result), new { id = submission.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Result(int id)
    {
        var userId = _userManager.GetUserId(User);
        var submission = await _db.Submissions
            .Include(x => x.Topic)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (submission is null)
        {
            return NotFound();
        }

        var messages = await _db.ChatMessages
            .Where(x => x.SubmissionId == id && x.UserId == userId)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync();

        return View(new SubmissionResultViewModel
        {
            Submission = submission,
            ChatMessages = messages
        });
    }

    private static int CountWords(string text)
    {
        return text.Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
