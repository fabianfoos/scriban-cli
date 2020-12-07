using System.Text.Json;
using Scriban.Runtime;

namespace ScribanCli
{
    internal class CustomTemplateContext : Scriban.TemplateContext
    {
        protected override IObjectAccessor GetMemberAccessorImpl(object target)
        {
            if (target is JsonElement json)
            {
                if (json.ValueKind == JsonValueKind.Object)
                    return JsonObjectAccessor.Default;
                else if (json.ValueKind == JsonValueKind.Array)
                    return JsonArrayAccessor.Default;
            }

            return base.GetMemberAccessorImpl(target);
        }
    }
}
