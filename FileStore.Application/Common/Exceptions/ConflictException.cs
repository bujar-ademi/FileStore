using FileStore.Application.Helpers;
using System;
using System.Collections.Generic;

namespace FileStore.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base() { }
        public ConflictException(string message, IDictionary<string, object> args = null) : base(TemplateHelper.FillTemplate(message, args)) { }
    }
}
