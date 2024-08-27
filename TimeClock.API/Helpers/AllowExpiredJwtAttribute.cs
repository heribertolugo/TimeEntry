namespace TimeClock.Api.Helpers;

public interface IAllowExpiredJwtAttribute { }

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
public class AllowExpiredJwtAttribute : Attribute, IAllowExpiredJwtAttribute
{

}
