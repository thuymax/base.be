using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;
using System.Reflection;

namespace SEEDONE.SERVICE.Interfaces.Repo.Business
{
    public interface IBaseRepo
    {
        /// <summary>
        /// Lấy toàn bộ danh sách thực thể T
        /// </summary>
        /// <returns>Danh sách thực thể T</returns>
        Task<List<T>> GetAsync<T>();

        /// <summary>
        /// Lấy thông tin một thực thể T theo id.   
        /// </summary>
        /// <param name="id">id khóa chính.</param>
        /// <returns>Thông tin thực thể T</returns>
        Task<T> GetByIdAsync<T>(object id);
        Task<T> GetByIdAsync<T>(IDbConnection cnn, object id);
        Task<T> GetByIdAsync<T>(IDbConnection cnn, Type type, object id);


        /// <summary>
        /// Lấy dữ liệu từ bảng theo điều kiện 1 trường cụ thể 
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="field">Tên trường</param>
        /// <param name="value">Giá trị</param>
        /// <param name="op">Toán tử</param>
        Task<List<T>> GetAsync<T>(string field, object value, string op = "=");

        /// <summary>
        /// Thêm dữ liệu
        /// </summary>
        /// <param name="entity">dữu liệu</param>
        Task<T> InsertAsync<T>(object entity);
        Task<object> InsertAsync(IDbTransaction transaction, Type type, object entity);

        /// <summary>
        /// Sửa dữ liệu
        /// </summary>
        /// <param name="entity">dữ liệu</param>
        /// <param name="fields">Danh sách các trường cập nhật</param>
        /// <returns></returns>
        Task<T> UpdateAsync<T>(object entity, string fields = null);
        Task<bool> UpdateAsync(object entity, string fields = null);
        Task<bool> UpdateAsync(IDbConnection cnn, object entity, string fields = null);
        Task<bool> UpdateMultiAsync(IDbConnection cnn, Type type, string updateField, object updateValue, IList ids);
        Task<bool> UpdateAsync(IDbTransaction transaction, Type type, object entity, string fields = null);
        Task<bool> UpdateMultiAsync(IDbTransaction transaction, Type type, string updateField, object updateValue, IList ids);

        /// <summary>
        /// Xóa dữu liệu
        /// </summary>
        Task<bool> DeleteAsync(object entity);
        /// <summary>
        /// Xóa dữu liệu
        /// </summary>
        Task<bool> DeleteAsync(IDbTransaction transaction, object entity);
        Task<bool> DeleteAsync(IDbTransaction transaction, Type type, object entity);

        IConfiguration GetConfiguration();

        /// <summary>
        /// Lấy dữ liệu combobox
        /// </summary>
        Task<IList> GetComboboxPaging(Type type, string colums, string filter, string sort);

        /// <summary>
        /// Lấy dữ liệu bảng
        /// </summary>
        Task<DAResult> GetDataTable(Type type, FilterTable filterTable);

        IDbConnection GetOpenConnection();
        void CloseConnection(IDbConnection cnn);

        /// <summary>
        /// Lấy dữ liệu chi tiết theo master id
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="config">cấu hình detail</param>
        /// <param name="returnType">kiểu dl trả vể</param>
        /// <param name="masterId">Giá trị kháo chính master</param>
        /// <returns></returns>
        Task<IList> GetRefByMasterAsync(IDbConnection cnn, IMasterRefAttribute config, Type returnType, object masterId);
        /// <summary>
        /// Kiểm tra trùng mã
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="model">dữ liệu</param>
        /// <param name="keyFields">thông tin các trường khóa</param>
        /// <param name="uniqueFields">thông tin các trường unique</param>
        /// <returns>true: bị trùng, false: k bị trùng</returns>
        Task<IList> HasDuplicateAsync(IDbConnection cnn, object model, List<PropertyInfo> keyFields, List<PropertyInfo> uniqueFields);

        /// <summary>
        /// Lấy dữ liệu theo tên bảng và cột trả ra
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="column"></param>
        /// <param name="field"></param>
        /// <param name="value">giá trị lọc</param>
        /// <param name="op">toán tử lọc</param>
        /// <returns>Mảng dữ liệu động</returns>
        Task<List<TEntity>> GetDynamicAsync<TEntity>(string column, string field, object value, string op = "=");
        Task<IList> GetDynamicAsync(Type type, string columns, string sorts);
    }
}
