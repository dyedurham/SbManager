using System;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;
using SbManager.Models.ViewModels;

namespace SbManager.InternalCommandHandlers
{
    public class ErrorStatusCodeHandler : IStatusCodeHandler
    {
        private readonly IResponseNegotiator responseNegotiator;

        public ErrorStatusCodeHandler(IResponseNegotiator responseNegotiator)
        {
            this.responseNegotiator = responseNegotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = new Negotiator(context);

            Error error = null;
            if (context.Items.ContainsKey("OnErrorException"))
            {
                var exception = context.Items["OnErrorException"] as Exception;
                error = new Error { ErrorMessage = exception.Message, FullException = exception.ToString() };
            }

            response.WithModel(new ErrorPageViewModel
            {
                Title = "Sorry, something went wrong",
                Summary = error == null ? "An unexpected error occurred." : error.ErrorMessage,
                Details = error == null ? null : error.FullException
            })
            .WithStatusCode(statusCode)
            .WithView("Error");

            var errorresponse = responseNegotiator.NegotiateResponse(response, context);
            context.Response = errorresponse;

        }
    }
    public class ErrorPageViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
    }
}