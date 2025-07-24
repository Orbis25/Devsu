namespace Domain.Options;

public class CorsConfigOption
{
    public List<string>? OriginsAllowed { get; set; }
    public List<string>? MethodsAllowed { get; set; }
}