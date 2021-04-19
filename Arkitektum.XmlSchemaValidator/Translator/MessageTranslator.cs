using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arkitektum.XmlSchemaValidator.Translator
{
    internal class MessageTranslator
    {
        public static string TranslateError(string message)
        {
            if (Translate(message, Translations.InvalidChild, out var translation))
                return $"{translation}{AddListOfPossibleElements(message)}.{AddOtherElements(message)}.";

            if (Translate(message, Translations.IncompleteContent, out translation))
                return $"{translation}{AddListOfPossibleElements(message)}.{AddOtherElements(message)}.";

            if (Translate(message, Translations.CannotContainText, out translation))
                return $"{translation}{AddListOfPossibleElements(message)}.{AddOtherElements(message)}.";

            if (Translate(message, Translations.EnumerationConstraintFailed, out translation))
                return translation;

            if (Translate(message, Translations.TextOnly, out translation))
                return translation;

            if (Translate(message, Translations.AttributeNotEqualFixedValue, out translation))
                return translation;

            if (Translate(message, Translations.TagMismatch, out translation))
                return translation;

            if (Translate(message, Translations.GlobalElementDeclared, out translation))
                return translation;

            if (Translate(message, Translations.ElementNotDeclared, out translation))
                return translation;

            return message;
        }

        public static string TranslateWarning(string message)
        {
            if (Translate(message, Translations.SchemaNotFound, out var translation))
                return translation;

            if (Translate(message, Translations.NoSchemaInfoElement, out translation))
                return translation;

            if (Translate(message, Translations.NoSchemaInfoAttribute, out translation))
                return translation;

            return message;
        }

        private static string AddListOfPossibleElements(string message)
        {
            var translation = string.Empty;
            var matches = Translations.ListOfPossibleElements.Regex.Matches(message);

            foreach (Match match in matches)
                translation += FormatMessage(Translations.ListOfPossibleElements.Template, match);

            return translation;
        }

        private static string AddOtherElements(string message)
        {
            var translation = string.Empty;
            var matches = Translations.OtherElements.Regex.Matches(message);

            foreach (Match match in matches)
                translation += FormatMessage(Translations.OtherElements.Template, match);

            return translation;
        }

        private static bool Translate(string message, Translation translation, out string output)
        {
            output = null;
            var match = translation.Regex.Match(message);

            if (!match.Success)
                return false;

            output = FormatMessage(translation.Template, match);
            return true;
        }

        private static string FormatMessage(string template, Match match)
        {
            var message = template;
            var values = GetGroupValues(match);

            foreach (var kvp in values)
                message = message.Replace($"{{{kvp.Key}}}", kvp.Value);

            return message;
        }

        private static IDictionary<string, string> GetGroupValues(Match match)
        {
            var values = new Dictionary<string, string>();

            foreach (Group group in match.Groups)
            {
                if (!string.IsNullOrWhiteSpace(group.Value))
                    values.Add(group.Name, group.Value);
            }

            return values;
        }
    }
}
