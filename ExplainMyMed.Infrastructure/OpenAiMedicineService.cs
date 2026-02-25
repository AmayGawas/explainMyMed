using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ExplainMyMed.Infrastructure;

public class OpenAiMedicineService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiMedicineService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"];
    }

    public async Task<string> GetMedicineInfo(string medName)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return "Groq error: missing API key. Ensure configuration key 'Groq:ApiKey' is set.";
        }

        var prompt = $"""
Return ONLY valid JSON. No markdown. No explanations.:
manufacturer,
usedFor,
dosage,
commonSideEffect,
isSideEffectCommon,
severeReactions,
isSuitableForPregnantWomen,
isSuitableForKidneyProblem,
isAllergyInducing,
isSafeWithAlcohol,
isSteroidal,
isSafeWithBPMed,
sourceOfInfo

Medicine: {medName}
""";

        var requestBody = new
        {
            model = "openai/gpt-oss-120b",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            var retryAfter = response.Headers.RetryAfter?.ToString();
            var headers = string.Join("; ", response.Headers.Select(h => $"{h.Key}={string.Join(',', h.Value)}"));
            return $"Groq error: {response.StatusCode} - {error} - RetryAfter:{retryAfter} - Headers:{headers}";
        }

        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}