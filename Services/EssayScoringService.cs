using System.Text.Json;
using IELTSWritingAI.Models;

namespace IELTSWritingAI.Services;

public class EssayScoringService : IEssayScoringService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IGeminiService _geminiService;

    public EssayScoringService(IGeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    public async Task<EssayScoreResult> ScoreEssayAsync(WritingTopic topic, string essayText, CancellationToken cancellationToken = default)
    {
        var prompt = $$"""
You are an IELTS Writing examiner.

Evaluate the essay using IELTS Writing band descriptors.
Return ONLY valid JSON. Do not include markdown fences or explanations outside JSON.

JSON schema:
{
  "overall": 6.5,
  "taskAchievement": 6.0,
  "coherenceCohesion": 6.5,
  "lexicalResource": 6.0,
  "grammarRangeAccuracy": 6.0,
  "feedback": "Short, practical feedback for the student."
}

Task type: {{topic.TaskType}}
Topic:
{{topic.TopicText}}

Essay:
{{essayText}}
""";

        var response = await _geminiService.GenerateContentAsync(prompt, cancellationToken);
        if (string.IsNullOrWhiteSpace(response))
        {
            return CreateDemoScore(essayText);
        }

        var json = ExtractJsonObject(response);
        var result = JsonSerializer.Deserialize<EssayScoreResult>(json, JsonOptions);
        return result is null || result.Overall <= 0 ? CreateDemoScore(essayText) : result;
    }

    public async Task<string> TutorChatAsync(Submission submission, IReadOnlyList<ChatMessage> recentMessages, string userMessage, CancellationToken cancellationToken = default)
    {
        var chatHistory = string.Join(Environment.NewLine, recentMessages.Select(x => $"{x.Role}: {x.Message}"));
        var prompt = $$"""
You are an IELTS Writing tutor. Help the student improve their essay.
Be specific, concise, and practical. If rewriting a sentence, explain why it is better.

Topic:
{{submission.Topic?.TopicText}}

Essay:
{{submission.EssayText}}

Scores:
Overall: {{submission.OverallScore}}
Task Achievement: {{submission.TaskAchievement}}
Coherence and Cohesion: {{submission.CoherenceCohesion}}
Lexical Resource: {{submission.LexicalResource}}
Grammar Range and Accuracy: {{submission.GrammarRangeAccuracy}}

Recent chat:
{{chatHistory}}

Student question:
{{userMessage}}
""";

        var response = await _geminiService.GenerateContentAsync(prompt, cancellationToken);
        return string.IsNullOrWhiteSpace(response)
            ? "Demo mode: add your Gemini API key to receive AI tutoring feedback. For now, try improving your thesis statement, topic sentences, and examples."
            : response.Trim();
    }

    private static EssayScoreResult CreateDemoScore(string essayText)
    {
        var wordCount = essayText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var baseScore = wordCount < 180 ? 5.5m : wordCount < 250 ? 6.0m : 6.5m;

        return new EssayScoreResult
        {
            Overall = baseScore,
            TaskAchievement = baseScore,
            CoherenceCohesion = baseScore,
            LexicalResource = baseScore,
            GrammarRangeAccuracy = baseScore - 0.5m,
            Feedback = "Demo score only. Configure Gemini:ApiKey to receive real AI feedback. Your essay should clearly answer the prompt, organize ideas into paragraphs, use specific examples, and reduce grammar errors."
        };
    }

    private static string ExtractJsonObject(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start < 0 || end <= start)
        {
            throw new InvalidOperationException("Gemini did not return a JSON object.");
        }

        return text[start..(end + 1)];
    }
}
