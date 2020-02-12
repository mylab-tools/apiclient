using System.Linq.Expressions;

namespace MyLab.ApiClient
{
    class ApiCallParameter
    {
        public InputParameterDescription Description { get; }
        public Expression ParameterExpr { get; }

        public ApiCallParameter(InputParameterDescription description, Expression parameterExpr)
        {
            Description = description;
            ParameterExpr = parameterExpr;
        }
    }
}