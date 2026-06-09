namespace IELTSWritingAI.Services;

public interface IGeminiService
{
    Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
}
