using IELTSWritingAI.Models;

namespace IELTSWritingAI.Services;

public interface IEssayScoringService
{
    Task<EssayScoreResult> ScoreEssayAsync(WritingTopic topic, string essayText, CancellationToken cancellationToken = default);
    Task<string> TutorChatAsync(Submission submission, IReadOnlyList<ChatMessage> recentMessages, string userMessage, CancellationToken cancellationToken = default);
}
