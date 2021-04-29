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

        public XmlSchemaSet GetXmlSchemaSet(string key)
        {
            return _schemaSets.TryGetValue(key, out var schemaSet) ? schemaSet : null;
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

        public void RebuildSchemaSet(string key)
        {
            _logger.LogInformation($"Gjenoppbygger XmlSchemaSet for '{key}'...");

            lock (_schemaSetLock)
            {
                var path = Path.GetFullPath(Path.Combine(_options.CacheFilesPath, key));
                DeleteSchemaSets(path);

                if (_options.SchemaStreams.TryGetValue(key, out var stream))
                {
                    _options.SchemaStreams.Remove(key);
                    _schemaSets.TryAdd(key, CreateSchemaSet(key, stream));
                }
                else if (_options.SchemaUris.TryGetValue(key, out var namespaceAndUri))
                {
                    _options.SchemaUris.Remove(key);
                    _schemaSets.TryAdd(key, CreateSchemaSet(key, namespaceAndUri.TargetNamespace, namespaceAndUri.SchemaUri));
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
            var xmlResolver = _options.CacheFiles ? new XmlFileCacheResolver(key, _options.CacheFilesPath, _options.CacheDurationDays) : new XmlUrlResolver();
            var xmlSchemaSet = new XmlSchemaSet { XmlResolver = xmlResolver };
            var xmlSchema = XmlSchema.Read(stream, null);
            stream.Seek(0, SeekOrigin.Begin);

            xmlSchemaSet.Add(xmlSchema);

            return CompileSchemaSet(xmlSchemaSet);
        }

        private XmlSchemaSet CreateSchemaSet(string key, string targetNamespace, string schemaUri)
        {
            var xmlResolver = _options.CacheFiles ? new XmlFileCacheResolver(key, _options.CacheFilesPath, _options.CacheDurationDays) : new XmlUrlResolver();
            var xmlSchemaSet = new XmlSchemaSet { XmlResolver = xmlResolver };

            xmlSchemaSet.Add(targetNamespace, schemaUri);

            return CompileSchemaSet(xmlSchemaSet);
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
