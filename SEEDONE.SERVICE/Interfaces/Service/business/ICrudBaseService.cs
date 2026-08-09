using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Cruds;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface ICrudBaseService<TKey, TEntity, TEntityDtoEdit> : IBaseService<TEntity>
        where TEntityDtoEdit : TEntity, IRecordState
    {
        /// <summary>
        /// Hàm này dùng để truy vấn theo kháo chính
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntityDtoEdit> GetEditAsync(TKey id);
        /// <summary>
        /// Cất dữ liệu thêm/sửa
        /// </summary>
        Task<TEntityDtoEdit> SaveAsync(SaveParameter<TEntityDtoEdit,TEntity> parameter);
        Task<TEntityDtoEdit> SaveAsync(IDbConnection cnn, SaveParameter<TEntityDtoEdit, TEntity> parameter);
        /// <summary>
        /// Cất dữ liệu thêm/sửa
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="saveParameter"></param>
        /// <returns></returns>
        Task SaveDataAsync(IDbTransaction transaction, SaveParameter<TEntityDtoEdit, TEntity> saveParameter);

        /// <summary>
        /// Xóa dữ liệu 
        /// </summary>
        Task<List<DeleteError>> DeleteAsync(DeleteParameter<TKey, TEntity> parameter);
        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        Task DeleteAsync(IDbConnection cnn, DeleteParameter<TKey, TEntity> parameter, TEntity mdoel);
        /// <summary>
        /// Cập nhật gái trị cho nhiều bản ghi
        /// </summary>
        /// <param name="updateField">trường nào</param>
        /// <param name="updateValue">giá trị gì</param>
        /// <param name="ids">tệp pk bản ghi effect</param>
        /// <returns></returns>
        Task<bool> UpdateMultiAsync(string updateField, object updateValue, IList ids);
        Task<bool> UpdateMultiAsync(IDbConnection cnn, string updateField, object updateValue, IList ids);
        Task<bool> UpdateMultiAsync(IDbTransaction transaction, string updateField, object updateValue, IList ids);
        /// <summary>
        /// Lấy full bảng
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<IList> GetListAsync(string sort);

        /// <summary>
        /// Lấy dữ liệu thêm mới
        /// </summary>
        /// <returns></returns>
        Task<TEntityDtoEdit> GetNewAsync(string param);
    }
}
