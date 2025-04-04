using FileStore.Application.Helpers;
using System;
using System.Collections.Generic;

namespace FileStore.Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base() { }

        public ForbiddenAccessException(string message, IDictionary<string, object> args = null) : base(TemplateHelper.FillTemplate(message, args)) { }
    }
}
