var builder = DistributedApplication.CreateBuilder(args);

// Agregar la API de HelpDesk al dashboard de Aspire
var api = builder.AddProject<Projects.HelpDesk_API>("helpdesk-api")
    .WithExternalHttpEndpoints();

builder.Build().Run();
