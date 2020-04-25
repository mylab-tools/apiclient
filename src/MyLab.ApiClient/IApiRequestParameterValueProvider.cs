using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyLab.ApiClient
{
    interface IApiRequestParameterValueProvider
    {
        object GetValue();
    }

    class SimpleApiRequestParameterValueProvider : IApiRequestParameterValueProvider
    {
        private readonly object _value;

        public SimpleApiRequestParameterValueProvider(object value)
        {
            _value = value;
        }
        public object GetValue()
        {
            return _value;
        }
    }

    class ExpressionBasedApiRequestParameterValueProvider : IApiRequestParameterValueProvider
    {
        private readonly Expression _valueProviderExpression;

        private static readonly IExpressionValueProvider[] ValueProviders =
        {
            new ConstantExpressionValueProvider(), 
            new MemberExpressionValueProvider(), 
        };

        /// <summary>
        /// Initializes a new instance of <see cref="ExpressionBasedApiRequestParameterValueProvider"/>
        /// </summary>
        public ExpressionBasedApiRequestParameterValueProvider(Expression valueProviderExpression)
        {
            _valueProviderExpression = valueProviderExpression;
        }

        public object GetValue()
        {
            return ExpressionValueProvidingTools.GetValue(_valueProviderExpression);
        }
    }

    static class ExpressionValueProvidingTools
    {
        private static readonly IExpressionValueProvider[] ValueProviders =
        {
            new ConstantExpressionValueProvider(),
            new MemberExpressionValueProvider(),
            new CallExpressionValueProvider(), 
            new NewExpressionValueProvider(), 
            new MemberInitExpressionValueProvider()
        };

        public static object GetValue(Expression expression)
        {
            var valProvider = ValueProviders.FirstOrDefault(p => p.Predicate(expression));

            if (valProvider == null)
                throw new NotSupportedException($"Expression type '{expression.NodeType}' not supported");

            return valProvider.GetValue(expression);
        }
    }

    interface IExpressionValueProvider
    {
        bool Predicate(Expression expression);

        object GetValue(Expression expression);
    }

    class ConstantExpressionValueProvider : IExpressionValueProvider
    {
        public bool Predicate(Expression expression)
        {
            return expression == null || expression.NodeType == ExpressionType.Constant;
        }

        public object GetValue(Expression expression)
        {
            return ((ConstantExpression) expression)?.Value;
        }
    }

    class MemberExpressionValueProvider : IExpressionValueProvider
    {
        public bool Predicate(Expression expression)
        {
            return expression.NodeType == ExpressionType.MemberAccess;
        }

        public object GetValue(Expression expression)
        {
            var ma = (MemberExpression) expression;

            var targetObject = ExpressionValueProvidingTools.GetValue(ma.Expression);

            switch (ma.Member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) ma.Member).GetValue(targetObject);
                case MemberTypes.Method:
                    return ((PropertyInfo)ma.Member).GetValue(targetObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)ma.Member).GetValue(targetObject);
                default:
                    throw new NotSupportedException($"Member access '{ma.Member.MemberType}' not supported");
            }
        }
    }

    class CallExpressionValueProvider : IExpressionValueProvider
    {
        public bool Predicate(Expression expression)
        {
            return expression.NodeType == ExpressionType.Call;
        }

        public object GetValue(Expression expression)
        {
            var call = ((MethodCallExpression)expression);

            var targetObject = call.Object != null 
                ? ExpressionValueProvidingTools.GetValue(call.Object)
                : null;
            var args = call.Arguments
                .Select(ExpressionValueProvidingTools.GetValue)
                .ToArray();

            return call.Method.Invoke(targetObject, args);
        }
    }

    class NewExpressionValueProvider : IExpressionValueProvider
    {
        public bool Predicate(Expression expression)
        {
            return expression.NodeType == ExpressionType.New;
        }

        public object GetValue(Expression expression)
        {
            var ne = (NewExpression)expression;

            return ne.Constructor.Invoke(ne.Arguments.Select(ExpressionValueProvidingTools.GetValue).ToArray());
        }
    }

    class MemberInitExpressionValueProvider : IExpressionValueProvider
    {
        public bool Predicate(Expression expression)
        {
            return expression.NodeType == ExpressionType.MemberInit;
        }

        public object GetValue(Expression expression)
        {
            var mi = (MemberInitExpression)expression;

            var obj = ExpressionValueProvidingTools.GetValue(mi.NewExpression);
            foreach (var memberBinding in mi.Bindings)
            {
                if (memberBinding.BindingType != MemberBindingType.Assignment)
                    throw new NotSupportedException($"Member binding type '{memberBinding.BindingType}' not supported");
                var ma = (MemberAssignment)memberBinding;
                var memberValue = ExpressionValueProvidingTools.GetValue(ma.Expression);

                switch (memberBinding.Member.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo)memberBinding.Member).SetValue(obj, memberValue);
                        break;
                    case MemberTypes.Property:
                        ((PropertyInfo)memberBinding.Member).SetValue(obj, memberValue);
                        break;
                    default:
                        throw new NotSupportedException($"Member type '{memberBinding.Member.MemberType}' not supported");
                }
            }

            return obj;
        }
    }
}