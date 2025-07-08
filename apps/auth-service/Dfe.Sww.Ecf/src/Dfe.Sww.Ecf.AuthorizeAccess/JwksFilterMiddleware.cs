using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class JwksFilterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _allowedFields;

    public JwksFilterMiddleware(RequestDelegate next)
    {
        _next = next;
        
        // Define the allowed fields for GovUK OneLogin compatibility
        _allowedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "kty",
            "e", 
            "use",
            "kid",
            "n"
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/.well-known/jwks", StringComparison.OrdinalIgnoreCase))
        {
            // Capture the original response
            var originalBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            if (context.Response.StatusCode == 200 && 
                context.Response.ContentType?.Contains("application/json") == true)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                
                // Parse and filter the JWKS
                var jwks = JsonSerializer.Deserialize<JsonNode>(responseText);
                if (jwks?["keys"] is JsonArray keys)
                {
                    foreach (var key in keys)
                    {
                        if (key is JsonObject keyObj)
                        {
                            // Get all property names and remove those not in the allowed list
                            var propertyNames = keyObj.Select(kvp => kvp.Key).ToList();
                            foreach (var propertyName in propertyNames)
                            {
                                if (!_allowedFields.Contains(propertyName))
                                {
                                    keyObj.Remove(propertyName);
                                }
                            }
                        }
                    }
                }

                var filteredResponse = JsonSerializer.Serialize(jwks, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                context.Response.Body = originalBody;
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(filteredResponse);
                await context.Response.WriteAsync(filteredResponse);
            }
            else
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                context.Response.Body = originalBody;
                await responseBody.CopyToAsync(originalBody);
            }
        }
        else
        {
            await _next(context);
        }
    }
}