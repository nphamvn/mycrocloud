using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MockServer.WebMVC.Models.Common;

// [JsonConverter(typeof(AjaxResultConverter<object>))]
public class AjaxResult
{
    public ICollection<Error> Errors { get; set; } = new List<Error>();
    public bool Success
        => Errors.Count == 0;
    public void AddError(string message)
    {
        Errors.Add(new(message));
    }
}
public class AjaxResult<T> : AjaxResult where T : class
{
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

// public class AjaxResultConverter<T> : JsonConverter<AjaxResult<T>> where T : class
// {
//     public override AjaxResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//     {
//         throw new NotImplementedException();
//     }

//     public override void Write(Utf8JsonWriter writer, AjaxResult<T> value, JsonSerializerOptions options)
//     {
//         if (value.Success)
//         {
//             //write data
//             writer.WritePropertyName(nameof(value.Data));
//         } else {
//             //write error
//         }
//     }
// }
