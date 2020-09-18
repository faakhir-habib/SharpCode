using System.Collections.Generic;
using System.Linq;

namespace SharpCode
{
    internal static class Extensions
    {
        public static string Join(this IEnumerable<string> source, string separator) =>
            string.Join(separator, source);

        public static string FormatSourceCode(this string source) =>
            Utils.FormatSourceCode(source);

        public static string ToSourceCode(this AccessModifier accessModifier) =>
            accessModifier switch
            {
                AccessModifier.Internal => "internal",
                AccessModifier.PrivateInternal => "private internal",
                AccessModifier.Protected => "protected",
                AccessModifier.ProtectedInternal => "protected internal",
                AccessModifier.Public => "public",
                _ => "private"
            };

        public static string ToSourceCode(this Field field, bool formatted)
        {
            const string Template = "{access-modifier} {readonly} {type} {name};";
            var raw = Template
                .Replace("{access-modifier}", field.AccessModifier.ToSourceCode())
                .Replace("{readonly}", field.IsReadonly ? "readonly" : string.Empty)
                .Replace("{type}", field.Type)
                .Replace("{name}", field.Name);

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Parameter parameter)
        {
            const string Template = "{type} {name}";
            return Template
                .Replace("{type}", parameter.Type)
                .Replace("{name}", parameter.Name);
        }

        public static string ToSourceCode(this Constructor constructor, bool formatted)
        {
            const string Template = @"
{access-modifier} {name}({parameters}) { }";
            var raw = Template
                .Replace("{access-modifier}", constructor.AccessModifier.ToSourceCode())
                .Replace("{name}", constructor.ClassName)
                .Replace("{parameters}", constructor.Parameters.Select(param => param.ToSourceCode()).Join(", "));

            return formatted ? raw.FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Property property, bool formatted)
        {
            const string Template = @"
{access-modifier} {type} {name}
{
    get{getter}
    set{setter}
}
            ";

            var raw = Template
                .Replace("{access-modifier}", property.AccessModifier.ToSourceCode())
                .Replace("{type}", property.Type)
                .Replace("{name}", property.Name)
                .Replace("{getter}", property.Getter.Match(
                    some: (getter) => $" => {getter}{(getter.EndsWith("}") ? string.Empty : ";")}",
                    none: () => ";"
                ))
                .Replace("{setter}", property.Setter.Match(
                    some: (setter) => $" => {setter}{(setter.EndsWith("}") ? string.Empty : ";")}",
                    none: () => ";"
                ));

            return formatted ? raw.Trim().FormatSourceCode() : raw;
        }

        public static string ToSourceCode(this Class classData, bool formatted)
        {
            const string ClassTemplate = @"
namespace {namespace}
{
    {access-modifier} class {name}
    {
        {fields}
        {constructors}
        {properties}
    }
}
            ";

            // Do not format members separately, rather format the entire class, if requested
            var raw = ClassTemplate
                .Replace("{namespace}", classData.Namespace)
                .Replace("{access-modifier}", classData.AccessModifier.ToSourceCode())
                .Replace("{name}", classData.Name)
                .Replace("{fields}", classData.Fields.Select(field => field.ToSourceCode(false)).Join("\n"))
                .Replace("{constructors}", classData.Constructors.Select(ctor => ctor.ToSourceCode(false)).Join("\n"))
                .Replace("{properties}", classData.Properties.Select(property => property.ToSourceCode(false)).Join("\n"));

            return formatted ? raw.FormatSourceCode() : raw;
        }
    }
}