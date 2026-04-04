public class RequestContextMiddleware
{
  private readonly RequestDelegate _next;

  public RequestContextMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
  {
    // 1. User GUID (persistent)
    var userGuidString = context.Request.Cookies["user_guid"];

    Guid userGuid;
    
    if (!Guid.TryParse(userGuidString, out userGuid))
    {
      userGuid = Guid.NewGuid();

      context.Response.Cookies.Append("user_guid", userGuid.ToString(), new CookieOptions
      {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddYears(1)
      });
    }

    var sessionGuid = Guid.NewGuid();

    requestContext.UserGuid = userGuid;
    requestContext.SessionGuid = sessionGuid;

    await _next(context);
  }
}