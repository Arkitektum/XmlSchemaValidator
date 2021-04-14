using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Arkitektum.XmlSchemaValidator.Provider
{
    internal class XmlFileCacheResolver : XmlUrlResolver
    {
        private readonly string _key;
        private readonly string _cachePath;
        private readonly int _cacheDurationDays;

        public XmlFileCacheResolver(
            string key,
            string cachePath,
            int cacheDurationDays)
        {
            _key = key;
            _cachePath = cachePath;
            _cacheDurationDays = cacheDurationDays;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException("absoluteUri");

            if (absoluteUri.Scheme == "http" && (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream)))
            {
                var filePath = GetFilePath(absoluteUri);

                if (File.Exists(filePath))
                {
                    var lastUpdated = DateTime.Now.Subtract(File.GetLastWriteTime(filePath));
                    
                    if (lastUpdated.TotalDays < _cacheDurationDays)
                        return File.OpenRead(filePath);                   
                }
                    
                var request = WebRequest.Create(absoluteUri);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                if (memoryStream.Length > 0)
                {
                    using var fileStream = CreateFile(filePath);
                    memoryStream.CopyTo(fileStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }

                stream.Dispose();
                return memoryStream;
            }
            else
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }

        private string GetFilePath(Uri uri)
        {
            return Path.GetFullPath(Path.Combine(_cachePath, _key, uri.Host + uri.LocalPath));
        }

        private static FileStream CreateFile(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            return File.Create(filePath);
        }
    }
}
