using Arkitektum.XmlSchemaValidator.Models;
using Arkitektum.XmlSchemaValidator.Provider;
using Arkitektum.XmlSchemaValidator.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using SchemaValidator = Arkitektum.XmlSchemaValidator.Validator.XmlSchemaValidator;

namespace Arkitektum.XmlSchemaValidator.Config
{
    public static class ValidatorConfig
    {
        public static void AddXsdValidator(this IServiceCollection services, Action<XsdValidatorOptions> options)
        {
            services.Configure(options);
            services.AddSingleton<IXmlSchemaSetProvider, XmlSchemaSetProvider>();
            services.AddTransient<IXmlSchemaValidator, SchemaValidator>();
        }

        public static void UseXsdValidator(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices.GetService<IXmlSchemaSetProvider>();
            provider.CreateSchemaSets();
        }
    }
}
