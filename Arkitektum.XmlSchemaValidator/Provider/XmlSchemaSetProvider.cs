using Arkitektum.XmlSchemaValidator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Arkitektum.XmlSchemaValidator.Provider
{
    public class XmlSchemaSetProvider : IXmlSchemaSetProvider
    {
        private readonly object _schemaSetLock = new();
        private readonly IDictionary<string, XmlSchemaSet> _schemaSets = new Dictionary<string, XmlSchemaSet>();
        private readonly XsdValidatorOptions _options;
        private readonly ILogger<XmlSchemaSetProvider> _logger;

        public XmlSchemaSetProvider(
            IOptions<XsdValidatorOptions> options,
            ILogger<XmlSchemaSetProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public void CreateSchemaSets()
        {
            _logger.LogInformation("Laster og kompilerer XmlSchemaSets...");

            foreach (var kvp in _options.SchemaStreams)
            {
                var schemaSet = CreateSchemaSet(kvp.Key, kvp.Value);
                _schemaSets.TryAdd(kvp.Key, schemaSet);
            }

            foreach (var kvp in _options.SchemaUris)
            {
                var schemaSet = CreateSchemaSet(kvp.Key, kvp.Value.TargetNamespace, kvp.Value.SchemaUri);
                _schemaSets.TryAdd(kvp.Key, schemaSet);
            }
        }

        public XmlSchemaSet GetXmlSchemaSet(object key)
        {
            return _schemaSets.TryGetValue(key.ToString(), out var schemaSet) ? schemaSet : null;
        }

        public void RebuildSchemaSets()
        {
            _logger.LogInformation("Gjenoppbygger XmlSchemaSets...");

            lock (_schemaSetLock)
            {
                DeleteSchemaSets(_options.CacheFilesPath);
                CreateSchemaSets();
            }
        }

        public void RebuildSchemaSet(object key)
        {
            var keyStr = key.ToString();
            _logger.LogInformation($"Gjenoppbygger XmlSchemaSet for '{keyStr}'...");

            lock (_schemaSetLock)
            {
                var path = Path.GetFullPath(Path.Combine(_options.CacheFilesPath, keyStr));
                DeleteSchemaSets(path);

                if (_options.SchemaStreams.TryGetValue(keyStr, out var stream))
                {
                    _options.SchemaStreams.Remove(keyStr);
                    _schemaSets.TryAdd(keyStr, CreateSchemaSet(keyStr, stream));
                }
                else if (_options.SchemaUris.TryGetValue(keyStr, out var namespaceAndUri))
                {
                    _options.SchemaUris.Remove(keyStr);
                    _schemaSets.TryAdd(keyStr, CreateSchemaSet(keyStr, namespaceAndUri.TargetNamespace, namespaceAndUri.SchemaUri));
                }
            }
        }

        private void DeleteSchemaSets(string path)
        {
            if (!_options.CacheFiles)
                return;

            try
            {
                if (!Directory.Exists(path))
                    return;

                _logger.LogInformation($"Sletter XSD-filer fra {path}...");

                Directory.Delete(path, true);

                _schemaSets.Clear();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Kunne ikke slette XSD-filer fra {path}!");
            }
        }

        private XmlSchemaSet CreateSchemaSet(string key, Stream stream)
        {
            var xmlSchemaSet = new XmlSchemaSet { XmlResolver = GetXmlResolver(key) };
            var xmlSchema = XmlSchema.Read(stream, null);
            stream.Seek(0, SeekOrigin.Begin);

            xmlSchemaSet.Add(xmlSchema);

            return CompileSchemaSet(xmlSchemaSet);
        }

        private XmlSchemaSet CreateSchemaSet(string key, string targetNamespace, string schemaUri)
        {
            var xmlSchemaSet = new XmlSchemaSet { XmlResolver = GetXmlResolver(key) };

            xmlSchemaSet.Add(targetNamespace, schemaUri);

            return CompileSchemaSet(xmlSchemaSet);
        }

        private XmlResolver GetXmlResolver(string key)
        {
            if (_options.CacheFiles)
                return new XmlFileCacheResolver(key, _options.CacheFilesPath, _options.CacheDurationDays);

            return new XmlUrlResolver();
        }

        private XmlSchemaSet CompileSchemaSet(XmlSchemaSet xmlSchemaSet)
        {
            try
            {
                xmlSchemaSet.Compile();
                return xmlSchemaSet;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Kunne ikke kompilere XmlSchemaSet!");
                return null;
            }
        }
    }
}
