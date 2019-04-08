﻿using ChartsWebApi.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.Extensions
{
    public class JwtTokenSlidingExpirationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITokenCreationService tokenCreationService;

        public JwtTokenSlidingExpirationMiddleware(RequestDelegate next, ITokenCreationService tokenCreationService)
        {
            this.next = next;
            this.tokenCreationService = tokenCreationService;
        }

        public Task Invoke(HttpContext context)
        {
            // Preflight check 1: did the request come with a token?
            var authorization = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authorization == null || !authorization.ToLower().StartsWith("bearer") || string.IsNullOrWhiteSpace(authorization.Substring(6)))
            {
                // No token on the request
                return next(context);
            }

            // Preflight check 2: did that token pass authentication?
            var claimsPrincipal = context.Request.HttpContext.User;
            if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                // Not an authorized request
                // Extract the claims and put them into a new JWT
                return next(context);
            }
            // Call the next delegate/middleware in the pipeline
            return next(context);
        }
    }
}
