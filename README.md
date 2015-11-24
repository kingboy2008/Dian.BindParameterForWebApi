# Dian.BindParameterForWebApi

ASP.NET WEB API中，支持POST与PUT接口自动绑定多个参数实体类。
支持从路由读取参数，自动绑定至同名API接口参数。

#### MultiParametersAttribute    
Allows API methods to accept multiple parameters via a POST operation. If a method is marked。
a single object with a property for each parameter. Both JSON and standard query string posting is supported. The parameters can be of any type.
The default behavior of .NET Web API is to allow just 1 parameter via a POST operation.
Example:
```c#
Given this web API method:
[MultiParameters]
public string MyMethod(CustomObject param1, CustomObject param2, string param3) 
{
	... 
}

a client would pass either a JSON object in this format: 
{ param1: {...}, param2: {...}, param3: "..." }

or an encoded query string:
param1=...&param2=...&param3=...

To use this attribute, Call this method in WebApiConfig.cs in Register :
config.ParameterBindingRules.Insert(0, MultiParameterBinding.CreateBindingForMarkedParameters);
```

#### ComplexSingleParameterAttribute
当API接口只有一个参数时，自动绑定接口参数，能够将路由中的参数值绑定到同名接口参数。
仅限POST/PUT。
```c#
WEB API代码示例如下：
[SingleComplexParameter]
[Route("api/{Id}")]
public IHttpActionResult UrlMultiComplex(UserVm vm)
{
	return Ok();
}

Class UserVm{
	Public int Id {get;set;}
	Public string Account {get;set;}
}

客户端代码示例如下：
$.ajax({
	data: JSON.stringify({ account: "param1" }),
	url: "/api/1001",
	contentType: "application/json", method: "POST", processData: false
})

使用该属性, 请在WebApiConfig.cs的Register方法中添加这一行代码:
config.ParameterBindingRules.Insert(0, ComplexSingleParameterBinding.CreateBindingForMarkedParameters);
```	
#### ComplexMultiParametersAttribute
API接口有多个参数时，自动绑定接口参数，并能将路由中的参数值绑定到同名接口参数。
仅限POST/PUT。
```c#
WEB API代码示例如下：
[MultiComplexParameter]
[Route("api/{Id}")]
public IHttpActionResult UrlMultiComplex(int id, UserVm vm1, UserVm vm2)
{
	return Ok();
}

Class UserVm{
	Public int Id {get;set;}
	Public string Account {get;set;}
}

客户端代码示例如下：
$.ajax({
	data: JSON.stringify({ vm1: { account: "param1" }, vm2 : { account : "param2" }}),
	url: "/api/1001",
	contentType: "application/json", method: "POST", processData: false
})

使用该属性, 请在WebApiConfig.cs的Register方法中添加这一行代码:
config.ParameterBindingRules.Insert(0, ComplexMultiParameterBinding.CreateBindingForMarkedParameters);
```

Nuget
---------------------------------------------

Plesae see [https://www.nuget.org/packages/Dian.BindParameterForWebApi](https://www.nuget.org/packages/Dian.BindParameterForWebApi/)

Bug tracker
---------------------------------------------

Have a bug? Please create an issue here on GitHub!  
[https://github.com/mybbcat/Dian.BindParameterForWebApi/issues](https://github.com/mybbcat/Dian.BindParameterForWebApi/issues)

Authors
---------------------------------------------

*mybbcat@gmail.com*
