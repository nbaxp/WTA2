using System.Text.Json;
using WTA.Application.Extensions;

namespace WTA.Application.Abstractions.Json;

public class UnderlineJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToUnderline();
    }
}
