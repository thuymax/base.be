using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface ITypeService
    {
        /// <summary>
        /// Lấy thông tin khóa chính của dữ liệu
        /// </summary>
        List<PropertyInfo> GetKeyFields(Type type);
        /// <summary>
        /// Lấy thông tin khóa chính của dữ liệu
        /// </summary>
        PropertyInfo GetKeyField(Type type);
        /// <summary>
        /// Lấy tên bảng
        /// </summary>
        string GetTableName(Type type);
        /// <summary>
        /// Lấy tên bảng
        /// </summary>
        string GetMasterTableName(Type type);
        /// <summary>
        /// Lấy danh sách các trường dl lưu trong DB
        /// </summary>
        List<string> GetTableColumns(Type type);

        /// <summary>
        /// Lấy danh sách thuộc tính của model có cấu hình Attribute
        /// </summary>
        /// <typeparam name="TAttribute">Attribute gì</typeparam>
        /// <param name="type">Đối tượng kiểm tra</param>
        Dictionary<PropertyInfo, TAttribute> GetPropertys<TAttribute>(Type type)
            where TAttribute : Attribute;

        /// <summary>
        /// Lấy kiểu dữ liệu trong list
        /// </summary>
        Type GetTypeInList(Type listType);

        /// <summary>
        /// Có phải list không
        /// </summary>
        bool IsList(Type type);
    }
}
