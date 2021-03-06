using System;
using System.Collections.Generic;
using System.IO;

namespace Arkitektum.XmlSchemaValidator.Models
{
    public class XsdValidatorOptions
    {
        public Dictionary<string, Stream> SchemaStreams { get; } = new Dictionary<string, Stream>();        
        public Dictionary<string, (string TargetNamespace, string SchemaUri)> SchemaUris { get; } = new Dictionary<string, (string, string)>();
        public int CacheDurationDays { get; set; } = 30;
        public string CacheFilesPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public bool CacheFiles { get; set; } = true;

        public void AddSchema(object key, Stream xsdStream)
        {
            SchemaStreams.TryAdd(key.ToString(), xsdStream);
        }

        public void AddSchema(object key, string targetNamespace, string schemaUri)
        {
            SchemaUris.TryAdd(key.ToString(), (targetNamespace, schemaUri));
        }
    }
}
