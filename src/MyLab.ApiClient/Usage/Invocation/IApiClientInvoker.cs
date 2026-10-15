using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Usage.Invocation;

/// <summary>
/// Defines an interface for invoking API client methods based on a contract interface.
/// </summary>
/// <typeparam name="TContract">
/// The type of the contract interface that specifies the API methods to be invoked.
/// </typeparam>
/// <remarks>
/// This interface provides the capability to create and configure API invocation expressions 
/// for methods defined in the specified contract interface. It serves as an abstraction for 
/// managing API interactions, including request preparation and response handling.
/// </remarks>
public interface IApiClientInvoker<TContract>
{
    /// <summary>   
    /// Creates an API invocation expression based on the provided contract method expression.
    /// </summary>
    /// <param name="expression">
    /// An expression representing a method of the contract interface <typeparamref name="TContract"/> 
    /// that defines the API call to be invoked.
    /// </param>
    /// <returns>
    /// An object implementing <see cref="IApiInvocationExpression{TContract}"/> that allows further configuration 
    /// and execution of the API invocation.
    /// </returns>
    IApiInvocationExpression<TContract> ForExpression(Expression<Func<TContract, Task>> expression);
}