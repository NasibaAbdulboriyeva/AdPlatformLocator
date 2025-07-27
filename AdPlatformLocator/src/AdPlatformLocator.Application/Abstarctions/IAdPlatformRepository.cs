using AdPlatformLocator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdPlatformLocator.Application.Abstarctions
{
  
    public interface IAdPlatformRepository
    {
        void OverWriteAll(IEnumerable<AdPlatform> platforms);
        List<AdPlatform> GetAll();
    }

}
