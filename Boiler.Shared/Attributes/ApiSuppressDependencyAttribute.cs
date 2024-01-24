namespace Boiler.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ApiSuppressDependencyAttribute : Attribute
{
    public ApiSuppressDependencyAttribute(string fullName)
    {
        FullName = fullName;
    }

    public string FullName { get; set; }
}