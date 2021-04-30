# Xml Schema Validator

### Oppsett (Startup.cs)
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddXmlSchemaValidator(options =>
    {
        // XSD som Stream
        options.AddSchema(
            "oversendelse",
            xsdStream
        );

        // XSD som target namespace og schema-URI
        options.AddSchema(
            "plankart",
            "http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0",
            "http://skjema.geonorge.no/SOSITEST/produktspesifikasjon/Reguleringsplanforslag/5.0/reguleringsplanforslag-5.0_rev20210303.xsd"
        );

        options.CacheFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "XSD");
        options.CacheDurationDays = 14;
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseXmlSchemaValidator();
}
```

### Bruk
```csharp
private readonly IXsdValidator _xsdValidator;

public SchemaValidationService(IXsdValidator xsdValidator)
{
    _xsdValidator = xsdValidator;
}

public List<string> Validate(object key, Stream xmlStream)
{
   return _xsdValidator.Validate(key, xmlStream);
}
```
