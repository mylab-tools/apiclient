using System.Collections.Generic;

namespace DotAspectClient
{
    class InvokeParameters : Dictionary<string, object>
    {
        public InvokeParameters(string[] names, object[] values)
        {
            for (int i = 0; i < names.Length; i++)
            {
                Add(names[i], values[i]);
            }
        }

        public InvokeParameters()
        {
            
        }
    }
}