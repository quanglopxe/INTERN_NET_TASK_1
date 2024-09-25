﻿using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using Newtonsoft.Json;

namespace MilkStore.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseException.ErrorException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.ErrorDetail.ErrorCode, ex.ErrorDetail.ErrorMessage.ToString());
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Core.Constants.StatusCodes statusCode, string errorCode, object errorMessage)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = statusCode,
                Code = errorCode,
                Message = errorMessage
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
