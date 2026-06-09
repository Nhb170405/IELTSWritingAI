namespace IELTSWritingAI.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public Submission? Submission { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
