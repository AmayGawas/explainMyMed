using System.Net.Http.Headers;
using System.Text;
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
        _apiKey = configuration["Gemini:ApiKey"];
    }

    public async Task<string> GetMedicineInfo(string medName)
    {
        var prompt = $"""
Give medicine info strictly in JSON with fields:
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
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}"
        );

        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Gemini error: {response.StatusCode} - {error}";
        }

        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}