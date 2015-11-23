using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class ComplexSingleParameterBinding : HttpParameterBinding
	{
		public ComplexSingleParameterBinding(HttpParameterDescriptor descriptor)
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
            // read request body (query string or JSON)
            var parameters = ParseParametersFromBody(actionContext.Request);
		    if (parameters != null)
		    {
                // 从路由中获取参数及值，全部添加进入BODY中取得的参数列表中
                // 如果路由参数与BODY参数有重复，将只保留路由参数
                foreach (var routeParam in actionContext.ControllerContext.RouteData.Values)
                {
                    parameters.Remove(routeParam.Key);
                    parameters.Add(routeParam.Key, routeParam.Value.ToString());
		        }
		    }
  
		    if (parameters != null)
			{
				object paramValue;
				if (Descriptor.ParameterType == typeof(string))
					paramValue = parameters.FirstOrDefault().Value;
				else if (Descriptor.ParameterType.IsEnum)
					paramValue = Enum.Parse(Descriptor.ParameterType, parameters.FirstOrDefault().Value.ToString());
				else if (Descriptor.ParameterType.IsPrimitive || Descriptor.ParameterType.IsValueType)
				    // TODO: Are these conditions ok? I'd rather not have to check that the type implements IConvertible.
				    paramValue = Convert.ChangeType(parameters.FirstOrDefault().Value, Descriptor.ParameterType);
				else
				{
                    var jsonSerializerSettings = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter.SerializerSettings;
                    //将完整的参数字典类型，转化为JSON字符串
                    string stringValue = JsonConvert.SerializeObject(parameters, jsonSerializerSettings);
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
        /// Read parameters from the body into a collection.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string,object> ParseParametersFromBody(HttpRequestMessage request)
		{
            object result = null;
            MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
			if (contentType != null)
			{
                switch (contentType.MediaType)
                {
                    case "application/json":
                        string content = request.Content.ReadAsStringAsync().Result;
                        var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                        result = values.Aggregate(new Dictionary<string,object>(), (seed, current) =>
                        {
                            seed.Add(current.Key, current.Value == null ? "" : current.Value.ToString());
                            return seed;
                        });
                        break;

                    case "application/x-www-form-urlencoded":
                        var nvc = request.Content.ReadAsFormDataAsync().Result;
                        result = nvc.AllKeys.ToDictionary(k => k, k => nvc[(string)k]);
                        break;
                }
            }
		    return result as Dictionary<string, object>;
		}


		public static ComplexSingleParameterBinding CreateBindingForMarkedParameters(HttpParameterDescriptor descriptor)
		{
			// short circuit if action does not have this attribute
			if (!descriptor.ActionDescriptor.GetCustomAttributes<ComplexSingleParameterAttribute>().Any(x => x.Enabled))
				return null;

			// Only apply this binder on POST and PUT operations
			Collection<HttpMethod> supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;
			if (supportedMethods.Contains(HttpMethod.Post) || supportedMethods.Contains(HttpMethod.Put))
				return new ComplexSingleParameterBinding(descriptor);
            
			return null;
		}

	}
}