using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SEEDONE.SERVICE.Cruds;

namespace SEEDONE.SERVICE.Service.Business
{
    public class BookmarkTypeService : CrudBaseService<IBookmarkTypeRepo, Guid, BookmarkTypeEntity, BookmarkTypeDtoEdit>, IBookmarkTypeService
    {
        public BookmarkTypeService(IBookmarkTypeRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }
        protected override async Task ValidateDeleteAssync(IDbConnection cnn, DeleteParameter<Guid, BookmarkTypeEntity> parameter, BookmarkTypeEntity model)
        {
            var exist = await _repo.GetAsync<TimeSheetEntity>(nameof(TimeSheetEntity.bookmark_type_id), model.bookmark_type_id);
            if (exist?.Count > 0)
            {
                throw new BusinessException
                {
                    ErrorCode = ErorrCodes.Valdiate,
                    ErrorData = new ValidateResult
                    {
                        Type = ValidateResultType.Generation,
                        Data = new
                        {
                            Code = model.bookmark_type_code,
                            Data = exist.Select(x => x.time_sheet_code).ToList()
                        }
                    }
                };
            }
            else
            {
                var existDossier = await _repo.GetAsync<DossierEntity>(nameof(DossierEntity.bookmark_type_id), model.bookmark_type_id);
                if (existDossier?.Count > 0)
                {
                    throw new BusinessException
                    {
                        ErrorCode = ErorrCodes.Valdiate,
                        ErrorData = new ValidateResult
                        {
                            Type = ValidateResultType.Generation,
                            Data = new
                            {
                                Code = model.bookmark_type_code,
                                Data = existDossier.Select(x => x.dossier_code).ToList()
                            }
                        }
                    };
                }
            }
        }
    }
}
