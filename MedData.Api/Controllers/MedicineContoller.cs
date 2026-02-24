using Microsoft.AspNetCore.Mvc;
using ExplainMyMed.Infrastructure;

namespace MedData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicineController : ControllerBase
{
    private readonly OpenAiMedicineService _service;

    public MedicineController(OpenAiMedicineService service)
    {
        _service = service;
    }

    [HttpGet("{medName}")]
    public async Task<IActionResult> Get(string medName)
    {
        var result = await _service.GetMedicineInfo(medName);
        return Ok(result);
    }
}