namespace IELTSWritingAI.Models;

public class WritingTopic
{
    public int Id { get; set; }
    public WritingTaskType TaskType { get; set; }
    public string TopicText { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
