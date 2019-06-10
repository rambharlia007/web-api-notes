using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace web_api_notes.Helper
{
    /*
        First, the pipeline gets the IContentNegotiator service from the HttpConfiguration object. It also gets the list of media formatters from the HttpConfiguration.Formatters collection.

        Next, the pipeline calls IContentNegotiator.Negotiate, passing in:

        The type of object to serialize
        The collection of media formatters
        The HTTP request
        The Negotiate method returns two pieces of information:

        Which formatter to use
        The media type for the response
        If no formatter is found, the Negotiate method returns null, and the client receives HTTP error 406 (Not Acceptable).
     */
    public class ContentNegotiation : ApiController
    {
        public HttpResponseMessage GetProduct(int id)
        {
            var product = new Product()
            { Id = id, Name = "Gizmo", Category = "Widgets", Price = 1.99M };

            IContentNegotiator negotiator = this.Configuration.Services.GetContentNegotiator();

            ContentNegotiationResult result = negotiator.Negotiate(
                typeof(Product), this.Request, this.Configuration.Formatters);
            if (result == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                throw new HttpResponseException(response);
            }

            return new HttpResponseMessage()
            {
                Content = new ObjectContent<Product>(
                    product,                // What we are serializing 
                    result.Formatter,           // The media formatter
                    result.MediaType.MediaType  // The MIME type
                )
            };
            // This code is equivalent to the what the pipeline does automatically.
        }
    }

    internal class Product
    {
        public Product()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}