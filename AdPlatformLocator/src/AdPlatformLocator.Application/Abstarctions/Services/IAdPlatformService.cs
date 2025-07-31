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
        Task LoadPlatformsFromFile(Stream fileStream);
        IEnumerable<string> FindPlatformsForLocation(string location);
    }
}
