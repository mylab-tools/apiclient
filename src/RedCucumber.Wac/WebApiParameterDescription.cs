using System;
using System.Collections.Generic;
using System.Reflection;

namespace RedCucumber.Wac
{
    class WebApiParameterDescription
    {
        public string Name { get; set; }

        public WebApiParameterType Type { get; set; }

        public static WebApiParameterDescription Create(ParameterInfo parameter)
        {
            return new WebApiParameterDescription
            {
                Name = parameter.GetCustomAttribute<NameAttribute>()?.Name ?? parameter.Name,
                Type = parameter.GetCustomAttribute<ParameterTypeAttribute>()?.ParameterType ?? WebApiParameterType.Undefined
            };
        }
    }

    public enum WebApiParameterType
    {
        Undefined,
        Get,
        Payload,
        FormItem
    }

    internal class WebApiParameterDescriptions : Dictionary<string, WebApiParameterDescription>
    {
        public WebApiParameterDescriptions()
        {

        }

        public WebApiParameterDescriptions(IEnumerable<WebApiParameterDescription> descriptions)
        {
            if (descriptions == null) throw new ArgumentNullException(nameof(descriptions));

            foreach (var webApiMethodDescription in descriptions)
            {
                Add(webApiMethodDescription);
            }
        }

        public void Add(WebApiParameterDescription apiDescription)
        {
            Add(apiDescription.Name, apiDescription);
        }
    }
}