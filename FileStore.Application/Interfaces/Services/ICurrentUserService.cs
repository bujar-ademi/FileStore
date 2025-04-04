using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string ApiClientId { get; }
    }
}
