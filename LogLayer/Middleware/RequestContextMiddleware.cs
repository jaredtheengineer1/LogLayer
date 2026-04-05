public class RequestContextMiddleware
{
  private readonly RequestDelegate _next;

  public RequestContextMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  private CookieOptions CreateCookieOptions(bool isPersistent)
  {
    var options = new CookieOptions
    {
      HttpOnly = true,
      SameSite = SameSiteMode.Lax,
      Secure = true,
      Expires = isPersistent ? DateTimeOffset.UtcNow.AddYears(1) : DateTimeOffset.UtcNow.AddHours(1)
    };
    
    if (!isPersistent)
    {
      options.IsEssential = true; // Mark as essential for GDPR compliance
    }

    return options;
  }

  public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
  {
    // 1. User GUID (persistent)
    var userGuidString = context.Request.Cookies["user_guid"];

    Guid userGuid;
    
    if (!Guid.TryParse(userGuidString, out userGuid))
    {
      userGuid = Guid.NewGuid();
    context.Response.Cookies.Append("user_guid", userGuid.ToString(), CreateCookieOptions(true));
    }

    // 2. Session GUID (per session)
    var sessionGuidString = context.Request.Cookies["session_guid"];
    Guid sessionGuid;
    
    if(!Guid.TryParse(sessionGuidString, out sessionGuid))
    {
      sessionGuid = Guid.NewGuid();
    }
    context.Response.Cookies.Append("session_guid", sessionGuid.ToString(), CreateCookieOptions(false));

    

    requestContext.UserGuid = userGuid;
    requestContext.SessionGuid = sessionGuid;

    await _next(context);
  }
}