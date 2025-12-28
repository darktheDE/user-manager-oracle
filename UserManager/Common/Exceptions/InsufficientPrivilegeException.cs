namespace UserManager.Common.Exceptions;

/// <summary>
/// Exception khi người dùng không có đủ quyền hạn
/// </summary>
public class InsufficientPrivilegeException : Exception
{
    public string? RequiredPrivilege { get; }
    
    public InsufficientPrivilegeException() 
        : base("Bạn không có đủ quyền hạn để thực hiện chức năng này.")
    {
    }
    
    public InsufficientPrivilegeException(string message) 
        : base(message)
    {
    }
    
    public InsufficientPrivilegeException(string message, string requiredPrivilege) 
        : base(message)
    {
        RequiredPrivilege = requiredPrivilege;
    }
    
    public InsufficientPrivilegeException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
