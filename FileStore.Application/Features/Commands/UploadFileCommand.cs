using FileStore.Application.Common.Models.Responses;
using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileStore.Application.Features.Commands
{
    public class UploadFileCommand : IRequest<FileResponse>
    {
        public IFormFile File { get; set; }
        public string Directory { get; set; }
    }

    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, FileResponse>
    {
        private readonly IStorageFactory storageFactory;
        private readonly ICurrentUserService currentUserService;
        private readonly IApiClientRepository apiClientRepository;

        public UploadFileCommandHandler(IStorageFactory storageFactory, ICurrentUserService currentUserService, IApiClientRepository apiClientRepository)
        {
            this.storageFactory = storageFactory;
            this.currentUserService = currentUserService;
            this.apiClientRepository = apiClientRepository;
        }

        public async Task<FileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var apiClientId = Guid.Parse(currentUserService.ApiClientId);
            var apiClient = await apiClientRepository.GetByIdAsync(apiClientId);

            return await storageFactory.GetStorageService(apiClient).StoreFileAsync(apiClient, request.File);
        }
    }
}
