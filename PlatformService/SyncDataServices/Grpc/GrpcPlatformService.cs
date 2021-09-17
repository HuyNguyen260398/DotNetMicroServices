using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo repo;
        private readonly IMapper mapper;

        public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequests requests, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = repo.GetAllPlatforms();

            foreach (var plaform in platforms)
            {
                response.Platform.Add(mapper.Map<GrpcPlatformModel>(plaform));
            }

            return Task.FromResult(response);
        }
    }
}