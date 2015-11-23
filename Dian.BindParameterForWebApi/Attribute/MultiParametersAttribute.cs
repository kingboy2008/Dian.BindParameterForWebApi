using System;
using Dian.BindParameterForWebApi.Binding;

namespace Dian.BindParameterForWebApi.Attribute
{
	/// <summary>
	/// Use this attribute on API methods that need to support multiple POST parameters. See the comments in
	/// class <see cref="MultiParameterBinding"/> for how to enable support for multiple POST parameters.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MultiParametersAttribute : System.Attribute
	{

		/// <summary>
		/// Specifies whether to support multiple POST parameters. This is true by default.
		/// </summary>
		public bool Enabled { get; set; }



		/// <summary>
		/// Initializes a new instance of the <see cref="MultiParametersAttribute"/> class with <see cref="MultiParametersAttribute.Enabled"/> set to true.
		/// </summary>
		public MultiParametersAttribute()
		{
			Enabled = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiParametersAttribute"/> class with <see cref="MultiParametersAttribute.Enabled"/> set to the specified value.
		/// </summary>
		public MultiParametersAttribute(bool enabled)
		{
			Enabled = enabled;
		}

	}
}