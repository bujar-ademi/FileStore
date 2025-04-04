using FileStore.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileStore.Application.Common.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, IDictionary<string, object> args) : base(TemplateHelper.FillTemplate(message, args)) { }
        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
