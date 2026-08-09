using System;
using Microsoft.Extensions.Configuration;
using SEEDONE.REPO.Mysql;
using SEEDONE.REPO.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Repo.Master;

namespace SEEDONE.REPO.Repo.Master
{
    public class MasterBaseRepo : BaseRepo, IMasterBaseRepo
    {
        public MasterBaseRepo(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
        }

        protected override IDataBaseProvider CreateProvider(string connectionString)
        {
            var masterConnection = this.GetConfiguration().GetConnectionString("MasterConnection");
            if (string.IsNullOrWhiteSpace(masterConnection))
            {
                throw new InvalidOperationException("MasterConnection chưa được cấu hình.");
            }

            return base.CreateProvider(masterConnection);
        }
    }
}

