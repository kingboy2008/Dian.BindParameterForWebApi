using System;

namespace Dian.BindParameterForWebApi.Attribute
{
    /// <summary>
    /// 以下场景可以使用本属性：
    /// 当API接口只有一个参数是实体类时，并将路由中的参数值绑定到Action参数实体类中时。
    /// 只支持POST/PUT方式。
    /// </summary>
    /// <remarks>
    /// WEB API代码示例如下：
    /// <code>
    /// [SingleComplexParameter]
    /// [Route("api/{Id}")]
    /// public IHttpActionResult UrlMultiComplex(UserVm vm)
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
	///		data: JSON.stringify({ account: "param1" }),
	///		url: "/api/1001",
	///		contentType: "application/json", method: "POST", processData: false
	///	})
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ComplexSingleParameterAttribute : System.Attribute
	{

		/// <summary>
		/// Specifies whether to support multiple POST parameters. This is true by default.
		/// </summary>
		public bool Enabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexSingleParameterAttribute"/> class with <see cref="ComplexSingleParameterAttribute.Enabled"/> set to true.
        /// </summary>
        public ComplexSingleParameterAttribute()
		{
			Enabled = true;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexSingleParameterAttribute"/> class with <see cref="ComplexSingleParameterAttribute.Enabled"/> set to the specified value.
        /// </summary>
        public ComplexSingleParameterAttribute(bool enabled)
		{
			Enabled = enabled;
		}

	}
}