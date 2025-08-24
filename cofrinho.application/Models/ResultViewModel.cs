using Flunt.Notifications;

namespace cofrinho.application.Models;

public class ResultViewModel
{
    public ResultViewModel(IEnumerable<string> messages, bool success = true)
    {
        Messages = messages?.ToArray() ?? Array.Empty<string>();
        IsSuccess = success;
    }

    public string[] Messages { get; init; }
    public bool IsSuccess { get; init; }

    // Fábricas para casos comuns
    public static ResultViewModel Success(string message = "Sucesso")
        => new( new[] { message });

    public static ResultViewModel Error(string message)
        => new(new[] { message }, false);

    public static ResultViewModel Error(IEnumerable<string> messages)
        => new(messages, false);

    public static ResultViewModel ErrorsFromNotifications(IEnumerable<Notification> notifications)
        => new(notifications.Select(n => n.Message), false);
}

public class ResultViewModel<T> : ResultViewModel
{
    public ResultViewModel(
        T? data,
        IEnumerable<string> messages,
        bool success = true) : base(messages, success)
    {
        Data = data;
    }

    public T? Data { get; private set; }

    public static ResultViewModel<T> Success(T? data, string message = "")
    {
        if(data is null)
            return new(data, new[] { "Sucesso" });
        
        
        return new(data, null);

     
        
    }

    public static ResultViewModel<T> Error(string message)
        => new(default, new[] { message }, false);

    public static ResultViewModel<T> Error(IEnumerable<string> messages)
        => new(default, messages, false);

    public static ResultViewModel<T> ErrorsFromNotifications(IEnumerable<Notification> notifications)
        => new(default, notifications.Select(n => n.Message), false);
}