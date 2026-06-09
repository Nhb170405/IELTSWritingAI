using IELTSWritingAI.Models;

namespace IELTSWritingAI.ViewModels;

public class WritingPracticeViewModel
{
    public int TopicId { get; set; }
    public WritingTaskType TaskType { get; set; }
    public string TopicText { get; set; } = string.Empty;
    public string EssayText { get; set; } = string.Empty;
}

public class SubmissionResultViewModel
{
    public Submission Submission { get; set; } = new();
    public IReadOnlyList<ChatMessage> ChatMessages { get; set; } = Array.Empty<ChatMessage>();
}
