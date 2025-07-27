using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Domain.Entities;

namespace AdPlatformLocator.Infrastructure.Persistence
{

    public class InMemoryAdPlatformRepository : IAdPlatformRepository
    {
        private List<AdPlatform> _storage = new();

        public void OverWriteAll(IEnumerable<AdPlatform> platforms)
        {
            if (platforms == null)
            {
                throw new ArgumentNullException(nameof(platforms));

            }

            _storage = platforms.ToList();
        }

        public List<AdPlatform> GetAll()
        {
            return _storage.ToList();
        }
    }

}
