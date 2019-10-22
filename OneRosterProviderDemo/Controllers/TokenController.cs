using System;
using OneRosterProviderDemo.Models;
using Microsoft.AspNetCore.Mvc;
using OneRosterProviderDemo.Middleware;

namespace OneRosterProviderDemo.Controllers
{
    [Route("token")]
    public class TokenController : BaseController
    {
        public TokenController(ApiContext _db) : base(_db)
        {
        }

        [HttpGet]
        public IActionResult GetToken()
        {
            var verification = OAuth.Verify(HttpContext, db);
            if (verification != 0)
            {
                return StatusCode(verification);
            }
            string token = OAuth.GenerateBearerToken();
            var tokenEntry = new OauthToken()
            {
                Value = token,
                CreatedAt = DateTime.Now
            };

            db.OauthTokens.Add(tokenEntry);
            db.SaveChanges();
            return JsonOk($"{{ \"access_token\": \"{token}\" }}");
        }
    }
}