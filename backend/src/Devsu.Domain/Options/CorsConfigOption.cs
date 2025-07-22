namespace Domain.Options;

public class CorsConfigOption
{
    public List<string>? OriginsAllowed { get; }
    public List<string>? MethodsAllowed { get; }
}