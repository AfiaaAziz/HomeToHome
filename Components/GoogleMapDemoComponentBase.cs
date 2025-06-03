using Microsoft.AspNetCore.Components;

namespace HomeToHome.Components // ✅ This must match what you use in @inherits
{
    public class GoogleMapDemoComponentBase : ComponentBase
    {
        public string ApiKey { get; set; } = "AIzaSyBdXmtyD1S1OlF8Ise4y98LLhOT-Le55EQ";
    }
}
