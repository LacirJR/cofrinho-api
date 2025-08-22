namespace cofrinho.application.Models;

public class ResultViewModel
{
    public ResultViewModel(string message, bool success = true)
    {
        Message = message;
        IsSuccess = success;
    }

    public string Message { get; init; }
    public bool IsSuccess { get; init; }
    
    
    public static ResultViewModel Success() => new("Sucesso");
    public static ResultViewModel Error(string message) => new(message, false);
}

public class ResultViewModel<T> : ResultViewModel
{
    public ResultViewModel(T? data, string message = "", bool success = true) : base(message, success)
    {
        Data = data;
    }
    public T? Data { get; private set; }
    
    public static ResultViewModel<T> Success(T? data) => new(data);
    public static ResultViewModel<T> Error(string message) => new(default, message, false);
}