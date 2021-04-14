using System.Collections.Generic;
using System.IO;

namespace Arkitektum.XmlSchemaValidator.Validator
{
    public interface IXmlSchemaValidator
    {
        public List<string> Validate(string key, Stream xmlStream);
    }
}
