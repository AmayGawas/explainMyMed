public class MedicineInfoDto
{
    public string Manufacturer { get; set; }
    public string UsedFor { get; set; }
    public string Dosage { get; set; }
    public string CommonSideEffect { get; set; }
    public bool IsSideEffectCommon { get; set; }
    public string SevereReactions { get; set; }
    public bool IsSuitableForPregnantWomen { get; set; }
    public bool IsSuitableForKidneyProblem { get; set; }
    public bool IsAllergyInducing { get; set; }
    public bool IsSafeWithAlcohol { get; set; }
    public bool IsSteroidal { get; set; }
    public bool IsSafeWithBPMed { get; set; }
    public string SourceOfInfo { get; set; }
}