using Serilog.Context;

namespace PetCare360.API.Middleware
{
    

    // aqui vai garantir que todo logo que foi ferado durante uma requisicao carregue o mesmo indentidicador. Porque sem isso em um volume muito grande de chamada é impossivel sdaber quais linhas de log pertencem a cada requisicao
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            context.Response.Headers[HeaderName] = correlationId;

            
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}