using Arkitektum.XmlSchemaValidator.Translator;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Arkitektum.XmlSchemaValidator.Validator
{
    internal class XsdValidator
    {
        private const int ValidationErrorCountLimit = 1000;
        private readonly List<string> _schemaValidationResult;

        public XsdValidator()
        {
            _schemaValidationResult = new List<string>();
        }

        public List<string> Validate(Stream xmlStream, XmlSchemaSet xmlSchemaSet)
        {
            var xmlReaderSettings = SetupXmlValidation(xmlSchemaSet);

            Validate(xmlStream, xmlReaderSettings);

            return _schemaValidationResult;
        }

        private void Validate(Stream xmlStream, XmlReaderSettings xmlReaderSettings)
        {
            using var validationReader = XmlReader.Create(xmlStream, xmlReaderSettings);

            try
            {
                while (validationReader.Read())
                    if (_schemaValidationResult.Count >= ValidationErrorCountLimit)
                        break;
            }
            catch (XmlException exception)
            {
                _schemaValidationResult.Add(MessageTranslator.TranslateError(exception.Message));
            }
        }

        private XmlReaderSettings SetupXmlValidation(XmlSchemaSet xmlSchemaSet)
        {
            var xmlReaderSettings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            
            xmlReaderSettings.Schemas.Add(xmlSchemaSet);
            xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            xmlReaderSettings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessIdentityConstraints;
            xmlReaderSettings.ValidationEventHandler += ValidationCallBack;

            return xmlReaderSettings;
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            var prefix = $"Linje {args.Exception.LineNumber}, posisjon {args.Exception.LinePosition}: ";
            
            switch (args.Severity)
            {
                case XmlSeverityType.Error:
                    prefix += MessageTranslator.TranslateError(args.Message);
                    break;
                case XmlSeverityType.Warning:
                    prefix += MessageTranslator.TranslateWarning(args.Message);
                    break;
            }

            _schemaValidationResult.Add(prefix);
        }
    }
}
