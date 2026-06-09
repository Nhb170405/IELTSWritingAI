using Microsoft.AspNetCore.Identity;

namespace IELTSWritingAI.Models;

public class Submission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
    public int TopicId { get; set; }
    public WritingTopic? Topic { get; set; }
    public string EssayText { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public decimal OverallScore { get; set; }
    public decimal TaskAchievement { get; set; }
    public decimal CoherenceCohesion { get; set; }
    public decimal LexicalResource { get; set; }
    public decimal GrammarRangeAccuracy { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
