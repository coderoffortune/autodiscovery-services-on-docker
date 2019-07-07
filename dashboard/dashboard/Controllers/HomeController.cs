using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;
using dashboard.Models;
using Newtonsoft.Json;
using dashboard.Types;

namespace dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly string autodiscoveryLabelKey = "tools.autodiscovery";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Uri dockerUri = new Uri("unix:///var/run/docker.sock");
            DockerClient client = new DockerClientConfiguration(dockerUri).CreateClient();

            var containersListParameters = new ContainersListParameters()
            {
                All = true,                
            };

            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(containersListParameters);

            IEnumerable<Either<string, AutodiscoveryConfigModel>> autodiscovery = containers
                .Where(KeepRunningContainersWithAutodiscoveryData)
                .Select(GetAutodiscoveryData);

            List<AutodiscoveryConfigModel> tools = autodiscovery.Where(tool => tool.IsRight()).Select(tool => tool.right).ToList();
            List<string> errorMessages = autodiscovery.Where(tool => !tool.IsRight()).Select(tool => tool.left).ToList();

            return View(model: new HomeModel(){
                Tools = tools,
                ContainersCount = containers.Count,
                ErrorMessages = errorMessages
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Either<string, AutodiscoveryConfigModel> GetAutodiscoveryData(ContainerListResponse container)
        {
            container.Labels.TryGetValue("tools.autodiscovery", out string autodiscoveryUrl);

            WebClient webClient = new WebClient();

            try
            {
                string autodiscoveryData = webClient.DownloadString(autodiscoveryUrl);

                AutodiscoveryConfigModel autodiscoveryConfig = JsonConvert.DeserializeObject<AutodiscoveryConfigModel>(autodiscoveryData);

                return new Either<string, AutodiscoveryConfigModel>(autodiscoveryConfig);
            }
            catch (Exception ex)
            {
                return new Either<string, AutodiscoveryConfigModel>(ex.Message);
            }
            finally
            {
                webClient.Dispose();
            }
        }

        private bool KeepRunningContainersWithAutodiscoveryData(ContainerListResponse container)
        {
            return container.State == "running" && container.Labels.ContainsKey(autodiscoveryLabelKey);
        }
    }
}
