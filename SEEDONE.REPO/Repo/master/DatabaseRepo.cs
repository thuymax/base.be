using System;
using Microsoft.Extensions.Configuration;
using SEEDONE.SERVICE.Interfaces.Repo.Master;

namespace SEEDONE.REPO.Repo.Master
{
    public class DatabaseRepo : MasterBaseRepo, IDatabaseRepo
    {
        public DatabaseRepo(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
        }
    }
}

