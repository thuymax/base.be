using System;
using Microsoft.Extensions.Configuration;
using SEEDONE.SERVICE.Interfaces.Repo.Master;

namespace SEEDONE.REPO.Repo.Master
{
    public class DatabaseUserRepo : MasterBaseRepo, IDatabaseUserRepo
    {
        public DatabaseUserRepo(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
        }
    }
}

