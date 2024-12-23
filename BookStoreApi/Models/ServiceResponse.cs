namespace BookStoreApi.Models;

public class ServiceResponse<T>
{
    T? Data { get; set; }
    bool Success { get; set; } = true;
    string? Message { get; set; }
}
