using System.Reflection;

[assembly: CLSCompliant(true)]

namespace Reflection;

/// <summary>
/// Provides operations for working with reflection.
/// </summary>
public static class ReflectionOperations
{
    /// <summary>
    /// Gets the type name of the specified object.
    /// </summary>
    /// <param name="obj">The object to get type name from.</param>
    /// <returns>The type name of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string GetTypeName(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets the full type name of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get full name from.</typeparam>
    /// <returns>The full type name.</returns>
    public static string GetFullTypeName<T>()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets the assembly-qualified name of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get assembly-qualified name from.</typeparam>
    /// <returns>The assembly-qualified name.</returns>
    public static string GetAssemblyQualifiedName<T>()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all private instance fields of the specified object.
    /// </summary>
    /// <param name="obj">The object to get fields from.</param>
    /// <returns>An array of strings containing field names.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string[] GetPrivateInstanceFields(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all public static fields of the specified object.
    /// </summary>
    /// <param name="obj">The object to get fields from.</param>
    /// <returns>An array of strings containing field names.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string[] GetPublicStaticFields(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all interface details of the specified object.
    /// </summary>
    /// <param name="obj">The object to get interface details from.</param>
    /// <returns>An array of strings containing interface details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string?[] GetInterfaceDataDetails(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all constructor details of the specified object.
    /// </summary>
    /// <param name="obj">The object to get constructor details from.</param>
    /// <returns>An array of strings containing constructor details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string?[] GetConstructorsDataDetails(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all type member details of the specified object.
    /// </summary>
    /// <param name="obj">The object to get member details from.</param>
    /// <returns>An array of strings containing member details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string?[] GetTypeMembersDataDetails(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all method details of the specified object.
    /// </summary>
    /// <param name="obj">The object to get method details from.</param>
    /// <returns>An array of strings containing method details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string?[] GetMethodDataDetails(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    /// <summary>
    /// Gets all property details of the specified object.
    /// </summary>
    /// <param name="obj">The object to get property details from.</param>
    /// <returns>An array of strings containing property details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
    public static string?[] GetPropertiesDataDetails(object? obj)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }
}
