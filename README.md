# XSD Validator

### Oppsett (Startup.cs)
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddXsdValidator(options =>
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

        options.CacheFilesPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Reguleringsplanforslag\\XSD"));
        options.CacheDurationDays = 14;
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseXsdValidator();
}
```

### Bruk
```csharp
private readonly IXsdValidator _xsdValidator;

public SchemaValidationService(IXsdValidator xsdValidator)
{
    _xsdValidator = xsdValidator;
}

public List<string> Validate(string key, Stream xmlStream)
{
   return _xsdValidator.Validate(key, xmlStream);
}
```
