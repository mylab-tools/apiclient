using System;

namespace MyLab.ApiClient.Contracts;

/// <summary>
/// Represents an exception that is thrown when an API contract is invalid.
/// </summary>
public class InvalidApiContractException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidApiContractException"/>
    /// </summary>
    public InvalidApiContractException(string message) : base(message)
    {
            
    }
}