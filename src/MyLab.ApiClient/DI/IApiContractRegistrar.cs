using System;

namespace MyLab.ApiClient.DI;

/// <summary>
/// Registers api contract 
/// </summary>
[Obsolete]
public interface IApiContractRegistrar
{
    /// <summary>
    /// Registers api contract 
    /// </summary>
    void RegisterContract<TContract>(string httpClientName)
        where TContract : class;

    /// <summary>
    /// Registers api contract 
    /// </summary>
    void RegisterContract<TContract>()
        where TContract : class;
}