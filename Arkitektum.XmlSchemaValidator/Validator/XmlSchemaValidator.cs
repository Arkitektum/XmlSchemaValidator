using Arkitektum.XmlSchemaValidator.Provider;
using System.Collections.Generic;
using System.IO;

namespace Arkitektum.XmlSchemaValidator.Validator
{
    public class XmlSchemaValidator : IXmlSchemaValidator
    {
        private readonly IXmlSchemaSetProvider _schemaSetProvider;

        public XmlSchemaValidator(
            IXmlSchemaSetProvider schemaSetProvider)
        {
            _schemaSetProvider = schemaSetProvider;
        }

        public List<string> Validate(string key, Stream xmlStream)
        {
            var xmlSchemaSet = _schemaSetProvider.GetXmlSchemaSet(key);

            if (xmlStream == null || xmlSchemaSet == null)
                return new List<string>();

            return new XsdValidator().Validate(xmlStream, xmlSchemaSet);
        }
    }
}
