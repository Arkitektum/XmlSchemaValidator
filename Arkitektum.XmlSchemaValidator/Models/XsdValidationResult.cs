using System.Collections.Generic;
using System.Linq;

namespace Arkitektum.XmlSchemaValidator.Models
{
    public class XsdValidationResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasErrors => Errors.Any();
        public bool HasWarnings => Warnings.Any();
    }
}
