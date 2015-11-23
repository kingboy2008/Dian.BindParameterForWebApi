using System;
using Dian.BindParameterForWebApi.Binding;

namespace Dian.BindParameterForWebApi.Attribute
{
    /// <summary>
    /// 以下场景可以使用本属性：
    /// 当API接口有多个参数是实体类时，并需将路由中的参数值绑定到Action参数实体类中时。
    /// 只支持POST/PUT方式。
    /// </summary>
	/// <remarks>
    /// WEB API代码示例如下：
    /// <code>
    /// [MultiComplexParameter]
    /// [Route("api/{Id}")]
    /// public IHttpActionResult UrlMultiComplex(UserVm vm1,UserVm vm2)
    /// {
    ///     return Ok();
    /// }
    /// 
    /// Class UserVm{
    ///     Public int Id {get;set;}
    ///     Public string Account {get;set;}
    /// }
    /// </code>
    /// 客户端代码示例如下：
    /// <code>
    /// $.ajax({
	///		data: JSON.stringify({ vm1: { account: "param1" }, vm2 : { account : "param2" }}),
	///		url: "/api/1001",
	///		contentType: "application/json", method: "POST", processData: false
	///	})
    /// </code>
    /// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ComplexMultiParametersAttribute : System.Attribute
	{

		/// <summary>
		/// Specifies whether to support multiple POST parameters. This is true by default.
		/// </summary>
		public bool Enabled { get; set; }



		/// <summary>
		/// Initializes a new instance of the <see cref="MultiParametersAttribute"/> class with <see cref="MultiParametersAttribute.Enabled"/> set to true.
		/// </summary>
		public ComplexMultiParametersAttribute()
		{
			Enabled = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiParametersAttribute"/> class with <see cref="MultiParametersAttribute.Enabled"/> set to the specified value.
		/// </summary>
		public ComplexMultiParametersAttribute(bool enabled)
		{
			Enabled = enabled;
		}

	}
}