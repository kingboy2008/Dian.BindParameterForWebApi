using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Web.Http;

namespace WebApplication.Tests
{
    /// <summary>
    /// WEB API单元测试类的基类
    /// 集成测试HTTP请求与返回的操作，简化单元测试代码
    /// </summary>
    public abstract class BaseWebApiUnitTest
    {
        private string GetBaseAddress()
        {
            var r = new Random();
            var port = r.Next(9000, 40000);
            return string.Format("http://localhost:{0}/",port);
        }

        protected HttpResponseMessage InvokeGetRequest(string api,IDictionary<string,string> header = null)
        {
            using (var invoker = CreateMessageInvoker())
            {
                using (var cts = new CancellationTokenSource())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, GetBaseAddress() + api);
                    if (header != null)
                    {
                        foreach (var pair in header)
                        {
                            request.Headers.Add(pair.Key, pair.Value);
                        }
                    }
                    var result = invoker.SendAsync(request, cts.Token).Result;
                    Trace.WriteLine(string.Format("单元测试 - {0}，返回的正文如下：", api));
                    Trace.WriteLine(result.Content.ReadAsStringAsync().Result);
                    return result;
                }
            }
        }

        private HttpResponseMessage InvokePostRequest<TArguemnt>(HttpMethod method,string api, TArguemnt arg, IDictionary<string, string> header = null)
        {
            var invoker = CreateMessageInvoker();
            using (var cts = new CancellationTokenSource())
            {
                var request = new HttpRequestMessage(method, GetBaseAddress() + api);
                if (header != null)
                    foreach (var pair in header)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }

                request.Content = new ObjectContent<TArguemnt>(arg, new JsonMediaTypeFormatter());
                var result = invoker.SendAsync(request, cts.Token).Result;
                Trace.WriteLine(string.Format("单元测试 - {0}，返回的正文如下：", api));
                Trace.WriteLine(result.Content.ReadAsStringAsync().Result);
                return result;
            }
        }

        protected HttpResponseMessage InvokePostRequest<TArguemnt>(string api, TArguemnt arg, IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Post, api, arg, header);
        }

        protected HttpResponseMessage InvokeDeleteRequest<TArguemnt>(string api, TArguemnt arg, IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Delete, api, arg, header);
        }

        protected HttpResponseMessage InvokePutRequest<TArguemnt>(string api, TArguemnt arg, IDictionary<string, string> header = null)
        {
            return InvokePostRequest(HttpMethod.Put, api, arg, header);
        }

        protected delegate void InvokeCallback(HttpConfiguration config);

        private HttpMessageInvoker CreateMessageInvoker()
        {
            var config = new HttpConfiguration();

            var invokeCallback = new InvokeCallback(Target);
            invokeCallback.Invoke(config);

            var server = new HttpServer(config);
            var messageInvoker = new HttpMessageInvoker(server);
            return messageInvoker;
        }

        protected virtual void Target(HttpConfiguration config)
        {
            
        }
    }

}
