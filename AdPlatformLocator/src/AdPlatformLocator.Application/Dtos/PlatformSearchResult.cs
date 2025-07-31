using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdPlatformLocator.Application.Dtos
{
    public class PlatformSearchResult
    {
        public string Location { get; set; }
        public int Count { get; set; }
        public List<string> Platforms { get; set; }
    }
}
