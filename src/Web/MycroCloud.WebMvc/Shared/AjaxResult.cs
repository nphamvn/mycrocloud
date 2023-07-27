namespace MycroCloud.WeMvc;

public class AjaxResult
{
    public ICollection<Error> Errors { get; set; } = new List<Error>();
    public bool Succeeded
        => Errors.Count == 0;
    public void AddError(string message)
    {
        Errors.Add(new(message));
    }

    public static AjaxResult Success() {
        return new AjaxResult();
    }
    public static AjaxResult Error()
    {
        return new AjaxResult();
    }
}
public class AjaxResult<T> : AjaxResult where T : class
{
    public AjaxResult()
    {
        
    }
    public AjaxResult(T data)
    {
        Data = data;
    }
    public T Data { get; set; }
}

public class Error
{
    public Error()
    {

    }
    public Error(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
    public int ErrorCode { get; }
    public string ErrorMessage { get; }
    public IEnumerable<string> MemberNames { get; }
}
