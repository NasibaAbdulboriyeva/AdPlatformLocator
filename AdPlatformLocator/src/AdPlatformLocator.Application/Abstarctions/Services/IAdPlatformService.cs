using AdPlatformLocator.Application.Dtos;
using AdPlatformLocator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdPlatformLocator.Application.Abstarctions.Services
{
    public interface IAdPlatformService
    {
        void LoadFromFile(Stream fileStream);
        List<AdPlatformResponse> FindPlatformsForLocation(SearchRequest request);
    }
}
