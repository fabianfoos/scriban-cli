using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace ScribanCli
{
    internal sealed class JsonArrayAccessor : IListAccessor, IObjectAccessor
    {
        public static JsonArrayAccessor Default = new JsonArrayAccessor();

        private JsonArrayAccessor()
        {
        }

        public int GetLength(TemplateContext context, SourceSpan span, object target)
        {
            var json = (JsonElement)target;
            if (json.ValueKind == JsonValueKind.Array)
                return json.EnumerateArray().Count();

            return 0;
        }

        public object GetValue(TemplateContext context, SourceSpan span, object target, int index)
        {
            var json = (JsonElement)target;
            if (json.ValueKind == JsonValueKind.Array)
                return json.EnumerateArray().ElementAt(index);

            return null;
        }

        public void SetValue(TemplateContext context, SourceSpan span, object target, int index, object value)
        {
            throw new System.NotSupportedException();
        }

        public int GetMemberCount(TemplateContext context, SourceSpan span, object target)
        {
            // size
            return 1;
        }

        public IEnumerable<string> GetMembers(TemplateContext context, SourceSpan span, object target)
        {
            yield return "size";
        }

        public bool HasMember(TemplateContext context, SourceSpan span, object target, string member)
        {
            return member == "size";
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, object target, string member, out object value)
        {
            if (member == "size")
            {
                value = GetLength(context, span, target);
                return true;
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