using SEEDONE.SERVICE.Interfaces.Repo.Business;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Properties;
using Dapper;
using System.Collections;

namespace SEEDONE.REPO.Repo.Business
{
    public class BookmarkTypeRepo : BaseRepo, IBookmarkTypeRepo
    {
        public BookmarkTypeRepo(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
        }

    }
}
