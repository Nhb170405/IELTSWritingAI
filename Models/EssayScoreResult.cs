using System.Text.Json.Serialization;

namespace IELTSWritingAI.Models;

public class EssayScoreResult
{
    [JsonPropertyName("overall")]
    public decimal Overall { get; set; }

    [JsonPropertyName("taskAchievement")]
    public decimal TaskAchievement { get; set; }

    [JsonPropertyName("coherenceCohesion")]
    public decimal CoherenceCohesion { get; set; }

    [JsonPropertyName("lexicalResource")]
    public decimal LexicalResource { get; set; }

    [JsonPropertyName("grammarRangeAccuracy")]
    public decimal GrammarRangeAccuracy { get; set; }

    [JsonPropertyName("feedback")]
    public string Feedback { get; set; } = string.Empty;
}
