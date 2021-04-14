namespace Arkitektum.XmlSchemaValidator.Translator
{
    internal static class Translations
    {
        public static Translation InvalidChild = new Translation(
            @"^The element '(?<element>[^ ]*)' in namespace '(?<ns>[^ ]*)' has invalid child element '(?<childElement>[^ ]*)' in namespace '(?<childNs>[^ ]*)'. List of possible elements expected: '(?<posElements>.*?)' in namespace '(?<posNs>.*?)'",
            "Elementet '{element}' i navnerom '{ns}' har ugyldig barnelement '{childElement}' i navnerom '{childNs}'. Liste med mulige forventede elementer: '{posElements}' i navnerom '{posNs}'"
        );

        public static Translation IncompleteContent = new Translation(
            @"^The element '(?<element>[^ ]*)' in namespace '(?<ns>[^ ]*)' has incomplete content. List of possible elements expected: '(?<posElements>.*?)' in namespace '(?<posNs>.*?)'",
            "Elementet '{element}' i navnerom '{ns}' har ufullstendig innhold. Liste med mulige forventede elementer: '{posElements}' i navnerom '{posNs}'"
        );

        public static Translation CannotContainText = new Translation(
            @"^The element '(?<element>[^ ]*)' in namespace '(?<ns>[^ ]*)' cannot contain text. List of possible elements expected: '(?<posElements>.*?)' in namespace '(?<posNs>.*?)'",
            "Elementet '{element}' i navnerom '{ns}' kan ikke inneholde tekst. Liste med mulige forventede elementer: '{posElements}' i navnerom '{posNs}'"
        );

        public static Translation EnumerationConstraintFailed = new Translation(
            @"^The '(?<element>[^ ]*)' element is invalid - The value '(?<value>.*?)' is invalid according to its datatype '(?<datatype>.*?)'",
            "Elementet {element} er ugyldig. Verdien '{value}' er ugyldig i henhold til dens datatype {datatype}. Oppregningsbegrensningen feilet."
        );

        public static Translation TextOnly = new Translation(
            @"^The element '(?<element>[^ ]*)' cannot contain child element '(?<childElement>[^ ]*)' because the parent element's content model is text only.",
            "Elementet '{element}' kan ikke inneholde barnelement '{childElement}' fordi foreldreelementets innholdsmodell er kun tekst."
        );

        public static Translation TagMismatch = new Translation(
            @"^The '(?<startTag>.*?)' start tag on line (?<startLine>\d+) position (?<startPos>\d+) does not match the end tag of '(?<endTag>.*?)'. Line (?<endLine>\d+), position (?<endPos>\d+).$",
            "Linje {startLine}, posisjon {startPos}: Start-taggen '{startTag}' matcher ikke slutt-taggen '{endTag}' på linje {endLine}, posisjon {endPos}."
        );

        public static Translation SchemaNotFound = new Translation(
            @"^Cannot load the schema for the namespace '(?<ns>.*?)' - Could not find file '(?<file>.*?)'.$",
            "Kan ikke laste skjemaet for navnerom '{ns}' - kunne ikke finne filen '{file}'."
        );

        public static Translation GlobalElementDeclared = new Translation(
            @"^The global element '(?<element>[^ ]*)' has already been declared.$",
            "Det globale elementet '{element}' er allerede erklært."
        );

        public static Translation ElementNotDeclared = new Translation(
            @"^The '(?<element>[^ ]*)' element is not declared.$",
            "Elementet '{element}' er ikke erklært."
        );

        public static Translation NoSchemaInfoElement = new Translation(
            @"^Could not find schema information for the element '(?<element>[^ ]*)'.$",
            "Kunne ikke finne skjemainformasjon for elementet '{element}'."
        );

        public static Translation NoSchemaInfoAttribute = new Translation(
            @"^Could not find schema information for the attribute '(?<attribute>[^ ]*)'.$",
            "Kunne ikke finne skjemainformasjon for attributtet '{attribute}'."
        );

        public static Translation OtherElements = new Translation(
            @" as well as '(?<otherElements>.*?)' in namespace '(?<otherNs>.*?)'",
            " i tillegg til '{otherElements}' i navnerom '{otherNs}'"
        );
    }
}
