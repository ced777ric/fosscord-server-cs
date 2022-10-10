namespace Fosscord.Util.Models;

public class FieldErrorInner
{
    public string message { get; set; }
    public object code { get; set; }
}

public class FieldError
{
    public List<FieldErrorInner> _errors { get; set; }
}

public class FieldValidationError
{
    public FieldValidationError(Dictionary<string, FieldError> error)
    {
        code = 50035;
        message = "Invalid Form Body";
        errors = error;
    }
    
    public int code { get; set; }
    public string message { get; set; }
    public Dictionary<string, FieldError> errors { get; set; }
}