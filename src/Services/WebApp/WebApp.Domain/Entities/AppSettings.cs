namespace WebApp.Domain.Entities;

public class AppSettings
{
    public bool CheckFunctionExecutionLimitMemory { get; set; }
    public long? FunctionExecutionLimitMemoryBytes { get; set; }
    public bool CheckFunctionExecutionTimeout { get; set; }
    public int? FunctionExecutionTimeoutSeconds { get; set; }
    public bool FunctionUseNoSqlConnection { get; set; }
}