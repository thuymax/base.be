using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.Cruds;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Interfaces.Entities;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;

namespace SEEDONE.SERVICE.Service.Business
{
    public abstract class CrudBaseService<TRepo, TKey, TEntity, TEntityDtoEdit> : BaseService<TEntity, TRepo>, ICrudBaseService<TKey, TEntity, TEntityDtoEdit>
        where TEntityDtoEdit : TEntity, IRecordState
        where TRepo : IBaseRepo
    {
        protected readonly IContextService _contextService;
        protected readonly IServiceProvider _serviceProvider;

        protected readonly int MASTER_INDEX = -1;
        protected CrudBaseService(TRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
            _contextService = serviceProvider.GetRequiredService<IContextService>();
        }

        public async Task<List<DeleteError>> DeleteAsync(DeleteParameter<TKey, TEntity> parameter)
        {
            if (parameter == null)
            {
                return null;
            }

            var result = new List<DeleteError>();
            IDbConnection cnn = null;
            try
            {
                // Mở kết nối
                cnn = _repo.GetOpenConnection();

                // Xử lý từng bản ghi
                for (int i = 0; i < parameter.Models.Count; i++)
                {
                    var item = parameter.Models[i];
                    try
                    {
                        await this.DeleteAsync(cnn, parameter, item);
                    }
                    catch (BusinessException ex)
                    {
                        result.Add(new DeleteError
                        {
                            Index = i,
                            Code = ex.ErrorCode ?? "Business",
                            Data = ex.ErrorData ?? ex.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        var msg = "Exception";
                        msg += "|" + ex.ToString();

                        result.Add(new DeleteError
                        {
                            Index = i,
                            Code = "Exception",
                            Data = msg
                        });
                    }
                }
            }
            finally
            {
                _repo.CloseConnection(cnn);
            }
            return result;
        }

        public async Task DeleteAsync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity model)
        {
            // Before delete
            await this.BeforeDeleteAsync(cnn, parameter, model);

            await this.ValidateDeleteAssync(cnn, parameter, model);

            await this.DeleteDataAsync(cnn, parameter, model);

            await this.AfterDeleteAsync(cnn, parameter, model);

        }
        /// <summary>
        /// Xử lý trc khi xóa
        /// </summary>
        protected virtual async Task BeforeDeleteAsync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity model)
        {
            await Task.CompletedTask;
        }
        /// <summary>
        /// Kiểm tra nghiệp vụ trc khi xóa
        /// </summary>
        protected virtual async Task ValidateDeleteAssync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity model)
        {
            await Task.CompletedTask;
        }
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        protected virtual async Task DeleteDataAsync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity model)
        {
            using var transaction = cnn.BeginTransaction();
            try
            {
                //Xử lý trc khi xóa giữ liệu gốc 
                await this.BeforeDeleteDataAsync(transaction, parameter);
                // Delete
                await _repo.DeleteAsync(transaction, model);
                //Xử lý sau khi xóa giữ liệu gốc 
                await this.AfterDeleteDataAsync(transaction, parameter);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        /// <summary>
        /// Xử lý trc khi xóa giữ liệu gốc 
        /// </summary>
        protected virtual async Task BeforeDeleteDataAsync(IDbTransaction cnn, DeleteParameter<TKey, TEntity> parameter)
        {
            await Task.CompletedTask;
        }
        /// <summary>
        /// Xử lý sau khi xóa giữ liệu gốc 
        /// </summary>
        protected virtual async Task AfterDeleteDataAsync(IDbTransaction cnn, DeleteParameter<TKey, TEntity> parameter)
        {
            await Task.CompletedTask;
        }
        /// <summary>
        /// Xử lý sau khi xóa
        /// </summary>
        protected virtual async Task AfterDeleteAsync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity model)
        {
            await Task.CompletedTask;
        }
        public virtual async Task<TEntityDtoEdit> GetEditAsync(TKey id)
        {
            IDbConnection cnn = null;
            try
            {
                cnn = _repo.GetOpenConnection();

                return await this.GetEditAsync(cnn, id);
            }
            finally
            {
                _repo.CloseConnection(cnn);
            }
        }

        protected virtual async Task<TEntityDtoEdit> GetEditAsync(IDbConnection cnn, TKey id)
        {
            //master
            var model = await this.GetEditMasterAsync(cnn, id);
            if (model != null)
            {
                // Xử lý editversion
                this.ProcessEditVersion(model);

                //load chi tiết
                await this.GetEditDetailAsync(cnn, model);
            }
            //Validate sau khi lấy dữ liệu
            await this.ValdiateAfterEditAsync(cnn, model);

            return model;
        }
        protected virtual async Task GetEditDetailAsync(IDbConnection cnn, TEntityDtoEdit master)
        {
            var config = this.GetDetailAttribute();
            if (config?.Count > 0)
            {
                var keyField = _typeService.GetKeyField(typeof(TEntityDtoEdit));
                var masterId = keyField.GetValue(master);

                foreach (var item in config)
                {
                    var detail = await this.LoadRefByMasterAsync(cnn, item.Key, item.Value, masterId);
                    item.Key.SetValue(master, detail);
                }
            }
        }
        protected virtual async Task<IList> LoadRefByMasterAsync(IDbConnection cnn, PropertyInfo info, IMasterRefAttribute attr, object masterId)
        {
            var type = _typeService.GetTypeInList(info.PropertyType);
            var data = await _repo.GetRefByMasterAsync(cnn, attr, type, masterId);
            return data;
        }

        protected virtual Dictionary<PropertyInfo, DetailAttribute> GetDetailAttribute()
        {
            return _typeService.GetPropertys<DetailAttribute>(typeof(TEntityDtoEdit));
        }
        protected virtual void ProcessEditVersion(object model)
        {
            if (model is IRecordVersion)
            {
                var versionAttr = _typeService.GetPropertys<EditVersionAttribute>(model.GetType()).FirstOrDefault();
                var version = this.GenerateVersion(model);
                if (version != null)
                {
                    versionAttr.Key.SetValue(model, version);
                }
            }
        }

        protected virtual long? GenerateVersion(object entity)
        {
            var modelType = typeof(TEntityDtoEdit);
            var versionAttr = _typeService.GetPropertys<EditVersionAttribute>(modelType).FirstOrDefault();
            if (versionAttr.Key != null)
            {
                var attr = (EditVersionAttribute)versionAttr.Value;
                var pr = entity?.GetType().GetProperty(attr.DataField);
                if (pr != null)
                {
                    var value = pr.GetValue(entity);
                    if (value != null)
                    {
                        if (value is DateTime)
                        {
                            return ((DateTime)value).Ticks;
                        }
                        else if (value is long)
                        {
                            return (long)value;
                        }
                        else if (value is int)
                        {
                            return Convert.ToInt64(value);
                        }
                    }
                }
            }
            return null;
        }

        protected virtual async Task<TEntityDtoEdit> GetEditMasterAsync(IDbConnection cnn, object id)
        {
            var model = await _repo.GetByIdAsync<TEntityDtoEdit>(cnn, typeof(TEntityDtoEdit), id);
            return model;
        }
        /// <summary>
        /// Validate sau khi lấy dữ liệu GetEdit
        /// </summary>
        protected virtual async Task ValdiateAfterEditAsync(IDbConnection cnn, TEntityDtoEdit model)
        {
            await Task.CompletedTask;
        }

        public virtual async Task<IList> GetListAsync(string sort)
        {
            return await _repo.GetDynamicAsync(typeof(TEntity), "*", sort);
        }

        public async Task<TEntityDtoEdit> SaveAsync(SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            IDbConnection cnn = null;
            try
            {
                cnn = _repo.GetOpenConnection();

                return await this.SaveAsync(cnn, parameter);
            }
            finally
            {
                _repo.CloseConnection(cnn);
            }
        }

        public async Task SaveDataAsync(IDbTransaction transaction, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            //master
            await this.SubmitItemAsync(transaction, typeof(TEntity), parameter.Model);
            //detail
            await this.SaveDataDetailAsync(transaction, parameter);
        }

        protected virtual async Task SubmitItemAsync(IDbTransaction transaction, Type type, IRecordState model)
        {
            switch (model.state)
            {
                case ModelState.Insert:
                    await _repo.InsertAsync(transaction, type, model);
                    break;
                case ModelState.Update:
                    await _repo.UpdateAsync(transaction, type, model);
                    break;
                case ModelState.Delete:
                    await _repo.DeleteAsync(transaction, type, model);
                    break;
            }
        }

        protected virtual async Task SaveDataDetailAsync(IDbTransaction transaction, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            var items = this.GetDetailAttribute();
            if (items?.Count > 0)
            {
                var masterId = parameter.Id;
                foreach (var item in items)
                {
                    var detailData = (IList)item.Key.GetValue(parameter.Model);

                    //Xửu lý trc khi cất
                    var saveData = await this.ProcessDetailItemBeforeSaveAsync(transaction, item.Value, detailData, masterId);

                    await this.UpdateDetailDataAsync(transaction, item.Value, saveData);
                }
            }
        }
        /// <summary>
        /// Cập nhật dl chi tiết
        /// </summary>
        protected virtual async Task UpdateDetailDataAsync(IDbTransaction transaction, DetailAttribute attr, IList detailData)
        {
            if (detailData?.Count > 0)
            {
                foreach (IRecordState item in detailData)
                {
                    await this.SubmitItemAsync(transaction, attr.Type, item);
                }
            }
        }
        protected virtual async Task<IList> ProcessDetailItemBeforeSaveAsync(IDbTransaction transaction, DetailAttribute attr, IList data, object masterKey)
        {
            return await this.ProcessRefInfoBeforeSaveAsync(transaction, attr, data, masterKey);
        }

        /// <summary>
        /// Xử lý các bản ghi chi tiết trc khi cập nhật
        /// </summary>
        protected virtual async Task<IList> ProcessRefInfoBeforeSaveAsync(IDbTransaction transaction, DetailAttribute attr, IList data, object masterKey)
        {
            //Cập nhật kháo ngoại master cho detail
            if (data?.Count > 0 
                &&(masterKey is int 
                || masterKey is uint 
                || masterKey is long
                || masterKey is ulong))
            {
                var firstItem = data[0];
                var type = firstItem.GetType();
                var pr = type.GetProperty(attr.MasterKeyField);
                foreach (var item in data)
                {
                    if (!(item is IRecordState) || (item as IRecordState).state == ModelState.Insert)
                    {
                        pr.SetValue(item, masterKey);
                    }
                    this.ProcessHistoryInfo(item);
                }
            }
            return data;
        }

        public async Task<bool> UpdateMultiAsync(string updateField, object updateValue, IList ids)
        {
            IDbConnection cnn = null;
            try
            {
                cnn = _repo.GetOpenConnection();
                var data = await _repo.UpdateMultiAsync(cnn, typeof(TEntity), updateField, updateValue, ids);
                return data;
            }
            finally
            {
                _repo.CloseConnection(cnn);
            }
        }
        public async Task<bool> UpdateMultiAsync(IDbConnection cnn, string updateField, object updateValue, IList ids)
        {
            var data = await _repo.UpdateMultiAsync(cnn, typeof(TEntity), updateField, updateValue, ids);
            return data;
        }
        public async Task<bool> UpdateMultiAsync(IDbTransaction transaction, string updateField, object updateValue, IList ids)
        {
            var data = await _repo.UpdateMultiAsync(transaction, typeof(TEntity), updateField, updateValue, ids);
            return data;
        }

        public async Task<TEntityDtoEdit> SaveAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            if (parameter == null || parameter.Model == null)
            {
                return default;
            }
            // Xử lý dl trc khi cất
            await this.BeforeSaveAsync(cnn, parameter);

            await this.ValidateSaveAsync(cnn, parameter);

            using var transaction = cnn.BeginTransaction();
            try
            {
                await this.SaveDataAsync(transaction, parameter);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            await this.AfterSaveAsync(cnn, parameter);

            if (parameter.ReturnRecord)
            {
                var data = await this.GetReturnRecordAsync(cnn, parameter.Model);
                return data;
            }

            return default;
        }

        public virtual async Task<TEntityDtoEdit> GetReturnRecordAsync(IDbConnection cnn, TEntityDtoEdit model)
        {
            var keyField = _typeService.GetKeyField(typeof(TEntityDtoEdit));
            var masterId = keyField.GetValue(model);

            var data = await this.GetEditAsync(cnn, (TKey)masterId);
            return data;
        }


        protected virtual async Task AfterSaveAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            if (parameter.IsNotUpdateAutoId)
            {
                return;
            }

            var configs = _typeService.GetPropertys<AutoIdAttribute>(typeof(TEntity));
            if (configs == null || configs.Count == 0)
            {
                return;
            }
            if (parameter.Model.state == ModelState.Update)
            {
                return;
            }
            foreach (var item in configs)
            {
                var category = item.Value.CategoryID;
                var autoId = await this.LoadAutoId((int)category);
                if (item.Key.GetValue(parameter.Model) == null)
                {
                    continue;
                }
                // LẤy ra giá trị các trường được config autoID để update
                var code = item.Key.GetValue(parameter.Model).ToString();
                var isAutoIdUpdateRule = true;
                if (!string.IsNullOrEmpty(code) && autoId != null && (code == this.GenerateNewCode(autoId)) || isAutoIdUpdateRule)
                {
                    _ = await UpdateAutoIDAsync(cnn, code, autoId.ref_type_category_id);
                } 

            }
        }

        public virtual async Task<bool> UpdateAutoIDAsync(IDbConnection cnn, string code, int keyAutoID)
        {
            var autoID = await ParseAutoId(code);
            autoID.ref_type_category_id = keyAutoID;
            var iSuccess = await _repo.UpdateAsync(cnn, autoID, "prefix,value,suffix,length_of_value");
            return iSuccess;
        }

        protected virtual async Task<AutoIdEntity> ParseAutoId(string code, bool hasSuffix = false)
        {
            var autoId = new AutoIdEntity()
            {
                prefix = "",
                value = 0,
                suffix = "",
                length_of_value = 0
            };
            if (code == null)
            {
                return autoId;
            }
            string strPrefix = "";
            string strSuffix = "";
            string strValue = "";
            var i = 0;
            for (i = code.Length - 1; i >= 0; i--)
            {
                if (!int.TryParse(code.Substring(i, 1), out var tempChar))
                {
                    break;
                }
                strValue = code.Substring(i, 1) + strValue;
            }
            strPrefix = code.Substring(0, i + 1);

            int.TryParse(strValue, out var iValue);
            var iLengthOfValue = strValue.Length;

            autoId.prefix = strPrefix;
            autoId.value = iValue;
            autoId.suffix = strSuffix;
            autoId.length_of_value = iLengthOfValue;

            return autoId;
        }
        /// <summary>
        /// Đọc vào DB cấu hình lấy thông tin autoId
        /// </summary>
        protected virtual async Task<AutoIdEntity> LoadAutoId(int categoryId)
        {
            var autoId = await _repo.GetDynamicAsync<AutoIdEntity>("*", "ref_type_category_id", categoryId);
            return autoId?.FirstOrDefault();
        }

        protected virtual string GenerateNewCode(AutoIdEntity autoId)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(autoId.prefix))
            {
                sb.Append(autoId.prefix);
            }

            var value = (autoId.value + 1).ToString();
            if (autoId.length_of_value > 0)
            {
                var len = value.Length;
                if (len < autoId.length_of_value)
                {
                    sb.Append(new string('0', autoId.length_of_value - len));
                }
            }
            sb.Append(value);

            if (!string.IsNullOrEmpty(autoId.suffix))
            {
                sb.Append(autoId.suffix);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Kiểm tra nghiệp vụ trc khi thêm mới
        /// hàm này trả về list ValidateResult fail để dùng lại đc cho những chỗ kahcs như nhập khẩu
        /// </summary>
        public virtual async Task ValidateSaveAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            //Kiểm tra phập đủ thông tin bắt buộc không
            await this.ValidateSaveRequireAsync(cnn, parameter);

            // mode sửa -> Kiểm tra editversion
            //lệch version thì dừng kiểm tra luôn
            if (parameter.Model.state == ModelState.Update && parameter.Model is IRecordVersion)
            {
                await ValidateSaveVersionAsync(cnn, parameter);
            }
            //Kiểm tra trùng mã
            await ValidateSaveDuplicateAsync(cnn, parameter);
        }

        public virtual async Task ValidateSaveDuplicateAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            var ruleCode = ValidateCode.Duplicate.ToString();
            if (parameter.CheckIgnore(ruleCode))
            {
                return;
            }
            var type = typeof(TEntity);
            var model = parameter.Model;

            var uniqueFields = _typeService.GetPropertys<UniqueAttribute>(type);
            if (uniqueFields == null || uniqueFields.Count == 0)
            {
                return;
            }

            List<PropertyInfo> keyFields = null;
            if (model.state == ModelState.Update)
            {
                var temp = _typeService.GetPropertys<KeyAttribute>(type);
                if (temp == null)
                {
                    return;
                }
                keyFields = temp.Select(n => n.Key).ToList();
            }

            var uniqueProps = uniqueFields.Select(n => n.Key).ToList();
            var dup = await _repo.HasDuplicateAsync(cnn, parameter.Model, keyFields, uniqueProps);

            if (dup == null || dup.Count == 0)
            {
                return;
            }
            var duplidateResults = new List<DuplicateResult>();

            await this.ValidateUniqueDuplicateWhenSave(duplidateResults, uniqueProps, parameter, dup);

            if (duplidateResults.Count > 0)
            {
                throw new BusinessException
                {
                    ErrorCode = ErorrCodes.Valdiate,
                    ErrorData = new ValidateResult
                    {
                        Type = ValidateResultType.Error,
                        Code = ruleCode,
                        Data = duplidateResults
                    }
                };
            }
        }

        /// <summary>
        /// Validate cá thông tin unique duplicate khi save
        /// </summary>
        public virtual async Task ValidateUniqueDuplicateWhenSave(List<DuplicateResult> duplicateResults, List<PropertyInfo> uniqueProps, SaveParameter<TEntityDtoEdit, TEntity> parameter, IList dup)
        {
            var index = 0;
            foreach (var field in uniqueProps)
            {
                var res = (IList)dup[index];
                index++;
                if (res == null || res.Count == 0)
                {
                    res = await GetDataDuplicate(parameter, field.Name);
                }
                if (res == null || res?.Count == 0)
                {
                    continue;
                }

                var fieldArgumentConstructor = field.GetCustomAttributesData().FirstOrDefault(x => x.AttributeType == typeof(UniqueAttribute))?.ConstructorArguments;
                var fieldDisplay = "";
                if (fieldArgumentConstructor?.Count > 0)
                {
                    fieldDisplay = fieldArgumentConstructor[0].Value.ToString();
                }
                duplicateResults.Add(new DuplicateResult
                {
                    FieldValue = field.GetValue(parameter.Model),
                    FieldName = field.Name,
                    FieldDisplay = fieldDisplay
                });

            }
        }

        /// <summary>
        /// Validate thêm
        /// </summary>
        public virtual async Task<IList> GetDataDuplicate(SaveParameter<TEntityDtoEdit, TEntity> parameter, string fieldCheck)
        {
            return await Task.FromResult<IList>(null);
        }

        /// <summary>
        /// Kiểm tra edit version khi sửa
        /// </summary>
        public virtual async Task ValidateSaveVersionAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            var ruleCode = ValidateCode.EditVersion.ToString();
            if (!parameter.CheckIgnore(ruleCode))
            {
                if (!this.HasObsolete((IRecordVersion)parameter.Model, parameter.Old))
                {
                    await Task.CompletedTask;
                    return;
                }
                throw new BusinessException()
                {
                    ErrorCode = ErorrCodes.Valdiate,
                    ErrorData = new ValidateResult
                    {
                        Type = ValidateResultType.Error,
                        Code = ruleCode,
                    }
                };
            }
        }

        protected virtual bool HasObsolete(IRecordVersion model, TEntity entity)
        {
            var version = this.GenerateVersion(entity);
            var entityVersion = model.RecordVersion;
            if (version.HasValue && version > entityVersion)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra có nhập đầy đủ dl theo cấu hình entity không
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="parameter"></param>
        public virtual async Task<ValidateResult> ValidateSaveRequireAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            var dic = new Dictionary<string, List<ValidateRequireData>>();

            //check master
            this.ValdiateMasterWhenSave(ref dic, parameter.Model);

            // check detail
            this.ValdiateDetailWhenSave(ref dic, parameter.Model);

            if (dic.Count > 0)
            {
                throw new BusinessException()
                {
                    ErrorCode = ErorrCodes.Valdiate,
                    ErrorData = new ValidateResult
                    {
                        Type = ValidateResultType.Error,
                        Code = ValidateCode.Required.ToString(),
                        Data = dic
                    }
                };
            }
            return await Task.FromResult<ValidateResult>(null);
        }
        /// <summary>
        /// Validate master khi save
        /// </summary>
        /// <param name="validateRequireDatas"></param>
        /// <param name="model"></param>
        private void ValdiateMasterWhenSave(ref Dictionary<string, List<ValidateRequireData>> validateRequireDatas, TEntityDtoEdit model)
        {
            var masterType = typeof(TEntity);
            var prs = _typeService.GetPropertys<RequireAttribute>(typeof(TEntity));
            if (prs?.Count > 0)
            {
                var table = _typeService.GetTableName(masterType);
                var fields = this.ValidateRequired(table, MASTER_INDEX, model, prs.Select(n => n.Key));
                if (fields.Count > 0)
                {
                    validateRequireDatas.Add(table, new List<ValidateRequireData>
                    {
                        new ValidateRequireData
                        {
                            Index = MASTER_INDEX,
                            Fields = fields
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Validate detail khi save
        /// </summary>
        /// <param name="validateRequireDatas"></param>
        /// <param name="model"></param>
        private void ValdiateDetailWhenSave(ref Dictionary<string, List<ValidateRequireData>> validateRequireDatas, TEntityDtoEdit model)
        {
            var details = this.GetDetailAttribute();
            if (details == null)
            {
                return;
            }
            if (details.Count == 0)
            {
                return;
            }
            foreach (var detail in details)
            {
                var detailData = (IList)detail.Key.GetValue(model);
                if (detailData == null)
                {
                    continue;
                }
                if (detailData.Count == 0)
                {
                    continue;
                }
                var detailType = detailData[0].GetType();
                var detailRes = new List<ValidateRequireData>();
                var prsDetail = _typeService.GetPropertys<RequireAttribute>(detail.Value.Type);
                var table = _typeService.GetTableName(detail.Value.Type);

                for (int i = 0; i < detailData.Count; i++)
                {
                    var item = detailData[i];
                    if (item is IRecordState && (item as IRecordState).state == ModelState.Delete)
                    {
                        continue;
                    }
                    var fields = this.ValidateRequired(table, i, item, prsDetail.Select(n => n.Key));
                    if (fields.Count == 0)
                    {
                        continue;
                    }
                    detailRes.Add(new ValidateRequireData
                    {
                        Index = i,
                        Fields = fields
                    });
                }

                if (detailRes.Count > 0)
                {
                    validateRequireDatas[table] = detailRes;
                }
            }
        }

        public virtual List<string> ValidateRequired(string name, int index, object record, IEnumerable<PropertyInfo> prs)
        {
            var fields = new List<string>();
            if (prs?.Count() > 0)
            {
                foreach (var pr in prs)
                {
                    object value = pr.GetValue(record);
                    if (value == null || (value is Guid && Guid.Empty.Equals(value))
                        || (value is string && string.Empty.Equals(value))
                        || ((value is decimal || value is float || value is int || value is long) && 0.Equals(value))
                        )
                    {
                        fields.Add(pr.Name);
                    }
                }
            }
            return fields;
        }


        protected virtual async Task BeforeSaveAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            var master = parameter.Model;
            var detailConfig = this.GetDetailAttribute();
            object masterId = null;
            var keyField = _typeService.GetKeyField(typeof(TEntityDtoEdit));

            switch (master.state)
            {
                case ModelState.Insert:
                    //Nếu k có gái trị kháo chính thì tự sinh
                    masterId = this.ProcessNewKey(master, keyField);
                    break;
                case ModelState.Update:
                    masterId = keyField.GetValue(master);

                    //mặc định khi thêm mới là null
                    parameter.Old = await this.GetDtoEditAsync(cnn, (TKey)masterId);
                    break;
            }
            parameter.Id = masterId;

            //Xử lý thông tin lịch sử
            this.ProcessHistoryInfo(master);

            if (detailConfig?.Count > 0)
            {
                this.UpdateRefBeforeSave(detailConfig, masterId, master);
            }
        }

        /// <summary>
        /// Xử lý thông tin lịch sử
        /// </summary>
        public virtual void ProcessHistoryInfo(object data)
        {
            if (data is IRecordState)
            {
                var contextData = _contextService.Get();
                var now = DateTime.Now;
                switch (((IRecordState)data).state)
                {
                    case ModelState.Insert:
                        if (data is IRecordCreate)
                        {
                            ((IRecordCreate)data).created_date = now;
                            // Sử dụng FullName hoặc UserName thay vì CheckerName (đã bỏ)
                            ((IRecordCreate)data).created_by = !string.IsNullOrWhiteSpace(contextData.FullName) 
                                ? contextData.FullName 
                                : contextData.UserName ?? string.Empty;
                        }
                        break;
                    case ModelState.Update:
                        if (data is IRecordModify)
                        {
                            ((IRecordModify)data).modified_date = now;
                            // Sử dụng FullName hoặc UserName thay vì CheckerName (đã bỏ)
                            ((IRecordModify)data).modified_by = !string.IsNullOrWhiteSpace(contextData.FullName) 
                                ? contextData.FullName 
                                : contextData.UserName ?? string.Empty;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Cập nhật khóa chính master cho các dữ liệu liên quan: detail
        /// Cập nhật pk cho bản ghi chi tiết nếu chwua có
        /// </summary>
        /// <typeparam name="TAttribute">Kiểu cấu hình dl liên quan</typeparam>
        /// <param name="configs">cấu hình</param>
        /// <param name="masterKey">Khóa chính master</param>
        /// <param name="master">master</param>
        private void UpdateRefBeforeSave<TAttribute>(Dictionary<PropertyInfo, TAttribute> configs, object masterKey, object master, bool hasDetail = false)
            where TAttribute : IMasterRefAttribute
        {
            foreach (var config in configs)
            {
                var datas = (IList)config.Key.GetValue(master);
                if (datas == null)
                {
                    continue;
                }
                if (datas.Count == 0)
                {
                    continue;
                }
                var type = datas[0].GetType();
                var frField = type.GetProperty(config.Value.MasterKeyField);
                var pkField = _typeService.GetKeyField(type);

                if (frField == null)
                {
                    throw new MissingFieldException($"Thiếu cấu hình khóa ngoại {config.Value.Type.Name} với {master.GetType().Name}");
                }
                if (pkField == null)
                {
                    throw new MissingFieldException($"Thiếu cấu hình khóa chính {config.Value.Type.Name}");
                }

                Dictionary<PropertyInfo, DetailAttribute> detailConfig = null;
                if (hasDetail)
                {
                    detailConfig = _typeService.GetPropertys<DetailAttribute>(type);
                }

                foreach (IRecordState data in datas)
                {
                    this.UpdateRefDetailBeforeSave(data, detailConfig, masterKey, pkField, frField);
                }
            }
        }
        /// <summary>
        /// Update detail before save
        /// </summary>
        /// <param name="data"></param>
        /// <param name="detailConfig"></param>
        /// <param name="masterKey"></param>
        /// <param name="pkField"></param>
        /// <param name="frField"></param>
        private void UpdateRefDetailBeforeSave(IRecordState data, Dictionary<PropertyInfo, DetailAttribute> detailConfig, object masterKey, PropertyInfo pkField, PropertyInfo frField)
        {
            var key = this.ProcessNewKey(data, pkField);
            if (data.state == ModelState.Insert)
            {
                frField.SetValue(data, masterKey);
            }
            // xử lý detail
            if (detailConfig?.Count > 0)
            {
                this.UpdateRefBeforeSave(detailConfig, key, data);
            }
        }

        protected virtual async Task<TEntityDtoEdit> GetDtoEditAsync(IDbConnection cnn, TKey id)
        {
            var dtoEdit = await GetDataMasterDetailAsync(cnn, id);
            return dtoEdit;
        }

        protected virtual async Task<TEntityDtoEdit> GetDataMasterDetailAsync(IDbConnection cnn, TKey id)
        {
            var model = await this.GetEditMasterAsync(cnn, id);
            if (model != null)
            {
                await this.GetEditDetailAsync(cnn, model);
            }
            return model;
        }

        private object ProcessNewKey(object model, PropertyInfo keyField, bool force = false)
        {
            var value = keyField.GetValue(model);
            if ((keyField.PropertyType == typeof(Guid) || keyField.PropertyType == typeof(Guid?))
                && (force || value == null || ((Guid)value) == Guid.Empty))
            {
                value = Guid.NewGuid();
                keyField.SetValue(model, value);
            }

            return value;
        }

        public virtual async Task<TEntityDtoEdit> GetNewAsync(string param)
        {
            IDbConnection cnn = null;
            try
            {
                cnn = _repo.GetOpenConnection();

                return await this.GetNewAsync(cnn, param);
            }
            finally
            {
                _repo.CloseConnection(cnn);
            }
        }

        protected virtual async Task<TEntityDtoEdit> GetNewAsync(IDbConnection cnn, string param)
        {
            var entity = this.CreateEditModel();

            // khởi tạo các chi tiết - chỉ khởi tạo mảng rộng
            var details = this.GetDetailAttribute();
            if (details?.Count > 0)
            {
                foreach (var detail in details)
                {
                    this.GetDefaultRefData(entity, detail.Key);
                }
            }
            //Set AutoID
            await this.ProcessNewCode(cnn, entity);

            return entity;
        }
        /// <summary>
        /// Xử lý tự tăng mã cho đối tượng
        /// </summary>
        protected virtual async Task ProcessNewCode(IDbConnection cnn, object entity)
        {
            var configs = _typeService.GetPropertys<AutoIdAttribute>(typeof(TEntity));
            if (configs == null || configs.Count == 0)
            {
                return;
            }
            foreach (var item in configs)
            {
                var categoryId = (int)item.Value.CategoryID;
                var autuId = await this.LoadAutoId(categoryId);
                if (autuId == null)
                {
                    continue;
                }
                var newCode = await this.GenerateAutoNewCode(autuId, item);
                if (string.IsNullOrEmpty(newCode))
                {
                    continue;
                }
                item.Key.SetValue(entity, newCode);
            }
        }

        private async Task<string> GenerateAutoNewCode(AutoIdEntity autoId, KeyValuePair<PropertyInfo, AutoIdAttribute> autoIdItem)
        {
            var newCode = string.Empty;
            bool isExist = true;
            // Check trùng mã thì tăng giá trị đến khi không trùng nữa
            while (isExist)
            {
                newCode = this.GenerateNewCode(autoId);
                var entityExist = await _repo.GetAsync<TEntity>(autoIdItem.Key.Name, newCode);
                if (entityExist == null)
                {
                    break;
                }
                if (entityExist.Count == 0)
                {
                    break;
                }
                autoId.value += 1;
            }
            return newCode;
        }
        protected virtual IList GetDefaultRefData(TEntityDtoEdit entity, PropertyInfo detailInfo)
        {
            return null;
        }

        public virtual TEntityDtoEdit CreateEditModel()
        {
            var model = Activator.CreateInstance<TEntityDtoEdit>();
            var keyField = _typeService.GetKeyField(typeof(TEntityDtoEdit));
            var masterId = keyField.GetValue(model);
            if (masterId is Guid || masterId is Guid?)
            {
                keyField.SetValue(model, Guid.NewGuid());
            }

            return model;
        }
    }

}
