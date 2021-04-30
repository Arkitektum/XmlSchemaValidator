using System.Xml.Schema;

namespace Arkitektum.XmlSchemaValidator.Provider
{
    public interface IXmlSchemaSetProvider
    {
        void CreateSchemaSets();
        void RebuildSchemaSets();
        void RebuildSchemaSet(object key);
        XmlSchemaSet GetXmlSchemaSet(object key);
    }
}
