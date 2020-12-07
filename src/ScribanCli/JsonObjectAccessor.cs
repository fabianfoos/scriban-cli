using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace ScribanCli
{
    internal sealed class JsonObjectAccessor : IObjectAccessor
    {
        public static JsonObjectAccessor Default { get; } = new JsonObjectAccessor();

        private JsonObjectAccessor()
        {
        }
        public int GetMemberCount(TemplateContext context, SourceSpan span, object target)
        {
            var json = (JsonElement)target;
            if (json.ValueKind == JsonValueKind.Object)
                return json.EnumerateObject().Count();

            return 0;
        }

        public IEnumerable<string> GetMembers(TemplateContext context, SourceSpan span, object target)
        {
            var json = (JsonElement)target;
            if (json.ValueKind == JsonValueKind.Object)
                return json.EnumerateObject().Select(o => StandardMemberRenamer.Rename(o.Name));

            return Array.Empty<string>();
        }

        public bool HasMember(TemplateContext context, SourceSpan span, object target, string member)
        {
            return this.GetMembers(context, span, target).Contains(member);
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, object target, string member, out object value)
        {
            var json = (JsonElement)target;
            if (json.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in json.EnumerateObject())
                {
                    if (StandardMemberRenamer.Rename(prop.Name) == member)
                    {
                        value = prop.Value;
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        public bool TrySetValue(TemplateContext context, SourceSpan span, object target, string member, object value)
        {
            return false;
        }
    }
}