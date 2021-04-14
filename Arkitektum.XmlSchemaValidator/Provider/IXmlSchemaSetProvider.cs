using System.Xml.Schema;

namespace Arkitektum.XmlSchemaValidator.Provider
{
    public interface IXmlSchemaSetProvider
    {
        void CreateSchemaSets();
        void RebuildSchemaSets();
        void RebuildSchemaSet(string key);
        XmlSchemaSet GetXmlSchemaSet(string key);
    }
}
