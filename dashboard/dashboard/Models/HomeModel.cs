using System.Collections.Generic;
using Docker.DotNet.Models;

namespace dashboard.Models
{
    public class HomeModel
    {
        public int ContainersCount { get; set; }

        public List<AutodiscoveryConfigModel> Tools { get; set; }

        public List<string> ErrorMessages { get; set; }
    }
}
