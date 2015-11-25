using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using Dian.BindParameterForWebApi.Attribute;
using Newtonsoft.Json;

namespace Dian.BindParameterForWebApi.Binding
{ 
    public class ComplexMultiParameterBinding : HttpParameterBinding
	{

		public ComplexMultiParameterBinding(HttpParameterDescriptor descriptor)
			: base(descriptor)
		{ }

		/// <summary>
		/// Parses the parameter value from the request body.
		/// </summary>
		/// <param name="metadataProvider"></param>
		/// <param name="actionContext"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
		{
			// read request body (query string or JSON) into name/value pairs
			NameValueCollection parameters = ParseParametersFromBody(actionContext.Request);

			// try to get parameter value from parsed body
			string stringValue = null;
			if (parameters != null)
				stringValue = parameters[Descriptor.ParameterName];
            
			// if not found in body, try reading query string
			if (stringValue == null)
			{
				var queryStringPairs = actionContext.Request.GetQueryNameValuePairs();
				if (queryStringPairs != null)
					stringValue = queryStringPairs
						.Where(kv => kv.Key == Descriptor.ParameterName)
						.Select(kv => kv.Value)
						.FirstOrDefault();
			}
            
            // 遍历路由中的参数，将其添加或替换api接口的参数实体类的字段或普通字段
		    stringValue = ParseParametersFromRoute(stringValue, actionContext.ControllerContext.RouteData.Values, Descriptor.ParameterName);     

			// if found, convert/deserialize the parameter and set the binding
			if (stringValue != null)
			{
				object paramValue;
				if (Descriptor.ParameterType == typeof(string))
					paramValue = stringValue;
				else if (Descriptor.ParameterType.IsEnum)
					paramValue = Enum.Parse(Descriptor.ParameterType, stringValue);
				else if (Descriptor.ParameterType.IsPrimitive || Descriptor.ParameterType.IsValueType)
				    // TODO: Are these conditions ok? I'd rather not have to check that the type implements IConvertible.
				    paramValue = Convert.ChangeType(stringValue, Descriptor.ParameterType);
				else
				{
                    var jsonSerializerSettings = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter.SerializerSettings;
                    // when deserializing an object, pass in the global settings so that custom converters, etc. are honored
                    paramValue = JsonConvert.DeserializeObject(stringValue, Descriptor.ParameterType, jsonSerializerSettings);
                }

				// Set the binding result here
				SetValue(actionContext, paramValue);
			}

			// now, we can return a completed task with no result
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			tcs.SetResult(default(object));
			return tcs.Task;
		}
        
        /// <summary>
        ///  将路由中的字段数据到从BODY或URL中解析的数据集合中
        /// </summary>
        /// <param name="parameterValueFromBodyOrUrl">Body或Url中解析的参数</param>
        /// <param name="routeData"></param>
        /// <param name="targetParameter">目标参数</param>
        /// <returns></returns>
        private static string ParseParametersFromRoute(string parameterValueFromBodyOrUrl, IDictionary<string, object> routeData, string targetParameter)
        {
            Dictionary<string, object> parBu;
            if (Tool.TryParseJson(parameterValueFromBodyOrUrl, out parBu))
            {
                // 如果能成功转换,则是一个实体类，类似这样的字符串{account:"sdd"}，
                // 用路由的字段进行替换或添加
                foreach (var r in routeData)
                {
                    parBu.Remove(r.Key);
                    parBu.Add(r.Key, r.Value);
                }
                return JsonConvert.SerializeObject(parBu);
            }
            else
            {
                //如果不能转成JSON，那么说明不是实体类，而是一个值类型
                //使用路由的字段进行替换或添加
                foreach (var r in routeData)
                {
                    if (r.Key.Equals(targetParameter,StringComparison.OrdinalIgnoreCase))
                    {
                        return r.Value.ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Read parameters from the body into a collection.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private NameValueCollection ParseParametersFromBody(HttpRequestMessage request)
		{
			const string cacheKey = "MultiComplexParameterBinding";

			// try to read out of cache first
			object result;
			if (!request.Properties.TryGetValue(cacheKey, out result))
			{
				// if not in cache, get value from request body based on the content type
				MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
				if (contentType != null)
				{
					switch (contentType.MediaType)
					{
						case "application/json":
							// deserialize to Dictionary and convert to NameValueCollection
							string content = request.Content.ReadAsStringAsync().Result;
							var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
							result = values.Aggregate(new NameValueCollection(), (seed, current) =>
							{
								seed.Add(current.Key, current.Value == null ? "" : current.Value.ToString());
								return seed;
							});
							break;

						case "application/x-www-form-urlencoded":
							result = request.Content.ReadAsFormDataAsync().Result;
							break;
					}

					// write to cache
					if (result != null)
						request.Properties.Add(cacheKey, result);
				}
			}

			return result as NameValueCollection;
		}


        /// <summary>
        /// Returns a <see cref="ComplexMultiParameterBinding"/> object to use for the API method parameter specified.
        /// An object is only returned if the parameter's method is marked with <see cref="ComplexMultiParametersAttribute"/>,
        /// otherwise null is returned.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        /// <remarks>
        /// Call this method in WebApiConfig.cs in Register :
        ///		config.ParameterBindingRules.Insert(0, MultiComplexParameterBinding.CreateBindingForMarkedParameters);
        /// </remarks>
        public static ComplexMultiParameterBinding CreateBindingForMarkedParameters(HttpParameterDescriptor descriptor)
		{
			// short circuit if action does not have this attribute
			if (!descriptor.ActionDescriptor.GetCustomAttributes<ComplexMultiParametersAttribute>().Any(x => x.Enabled))
				return null;

			// Only apply this binder on POST and PUT operations
			Collection<HttpMethod> supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;
			if (supportedMethods.Contains(HttpMethod.Post) || supportedMethods.Contains(HttpMethod.Put) || supportedMethods.Contains(HttpMethod.Delete))
				return new ComplexMultiParameterBinding(descriptor);

			return null;
		}

	}
}