using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Filters
{
    public class FiltroDeException : ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroDeException> logger;

        public FiltroDeException(ILogger<FiltroDeException> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}