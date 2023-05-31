namespace MagicVilla_WebAPI.Configurations
{
    // we need to inject this middelware into program.cs
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder) =>
            applicationBuilder.UseMiddleware<GlobalExceptionHandlingMiddleware>();


    }
}
