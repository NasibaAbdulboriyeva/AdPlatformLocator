using AdPlatformLocator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdPlatformLocator.Application.Abstarctions
{
    public interface IAdFileParser
    {
        Task<IEnumerable<AdPlatform>> ParseAsync(Stream fileStream);
    }
}
