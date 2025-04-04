using FileStore.Application.Common.Models;
using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileStore.Application.Features.Query
{
    public class GetFileQuery : IRequest<FileModel>
    {
        public Guid Reference { get; set; }
        public string Hash { get; set; }
    }

    public class GetFileQueryHandler : IRequestHandler<GetFileQuery, FileModel>
    {
        private readonly IStorageFactory storageFactory;
        private readonly ICurrentUserService currentUserService;
        private readonly IApiClientRepository apiClientRepository;

        public GetFileQueryHandler(IStorageFactory storageFactory, ICurrentUserService currentUserService, IApiClientRepository apiClientRepository)
        {
            this.storageFactory = storageFactory;
            this.currentUserService = currentUserService;
            this.apiClientRepository = apiClientRepository;
        }
        public async Task<FileModel> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            var apiClientId = Guid.Parse(currentUserService.ApiClientId);
            var apiClient = await apiClientRepository.GetByIdAsync(apiClientId);
            var result = await storageFactory.GetStorageService(apiClient).DownloadAsync(apiClientId, request.Reference);

            return result;
        }
    }
}
