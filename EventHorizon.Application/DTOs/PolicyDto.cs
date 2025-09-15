namespace EventHorizon.Application.DTOs;

public class PolicyDto
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductRatePct { get; set; }
    public decimal MarginPct { get; set; }
    public decimal ThresholdMultiplier { get; set; }
    public decimal SplitAdvisor { get; set; }
    public decimal SplitIntroducer { get; set; }
    public decimal SplitManager { get; set; }
    public decimal SplitExec { get; set; }
}

public class PolicyCreateDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductRatePct { get; set; }
    public decimal MarginPct { get; set; }
    public decimal ThresholdMultiplier { get; set; } = 2.0m;
    public decimal SplitAdvisor { get; set; }
    public decimal SplitIntroducer { get; set; }
    public decimal SplitManager { get; set; }
    public decimal SplitExec { get; set; }
}

public class PolicyUpdateDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductRatePct { get; set; }
    public decimal MarginPct { get; set; }
    public decimal ThresholdMultiplier { get; set; }
    public decimal SplitAdvisor { get; set; }
    public decimal SplitIntroducer { get; set; }
    public decimal SplitManager { get; set; }
    public decimal SplitExec { get; set; }
}