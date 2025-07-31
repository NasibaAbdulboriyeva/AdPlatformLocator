using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdPlatformLocator.Application.Dtos
{
    public class UploadPlatformRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
