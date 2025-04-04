using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileStore.Application.Features.Commands
{
    public class DeleteFileCommand : IRequest<bool>
    {
        public Guid FileReference { get; set; }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
    {
        private readonly IStorageFactory storageFactory;
        private readonly ICurrentUserService currentUserService;
        private readonly IApiClientRepository apiClientRepository;

        public DeleteFileCommandHandler(IStorageFactory storageFactory, ICurrentUserService currentUserService, IApiClientRepository apiClientRepository)
        {
            this.storageFactory = storageFactory;
            this.currentUserService = currentUserService;
            this.apiClientRepository = apiClientRepository;
        }

        public async Task<bool> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var apiClientId = Guid.Parse(currentUserService.ApiClientId);
            var apiClient = await apiClientRepository.GetByIdAsync(apiClientId);

            return await storageFactory.GetStorageService(apiClient).DeleteFileAsync(request.FileReference, apiClient.Id);
        }
    }
}
