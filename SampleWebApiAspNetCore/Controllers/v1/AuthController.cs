using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Models;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthenticateController : BaseController
    {
        #region Property  
        /// <summary>  
        /// Property Declaration  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        private IConfiguration _config;
        private readonly testePAPContext _context;

        #endregion

        #region Contructor Injector  
        /// <summary>  
        /// Constructor Injection to access all methods or simply DI(Dependency Injection)  
        /// </summary>  
        public AuthenticateController(IConfiguration config,
            testePAPContext context)
        {
            _config = config;
            _context = context;
        }
        #endregion

        #region GenerateJWT  
        /// <summary>  
        /// Generate Json Web Token Method  
        /// </summary>  
        /// <param name="userInfo"></param>  
        /// <returns></returns>  
        [ApiExplorerSettings(IgnoreApi = true)]
        private string GenerateJSONWebToken(string login, string senha)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region AuthenticateUser  
        /// <summary>  
        /// Hardcoded the User authentication  
        /// </summary>  
        /// <param name="login"></param>  
        /// <returns></returns>  
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<bool> AuthenticateUser(string login, string senha)
        {

            Utilizador utilizador = _context.Utilizador
                .Where(u => u.Login == login && u.Senha == senha).FirstOrDefault();

            if (utilizador != null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Login Validation  
        /// <summary>  
        /// Login Authenticaton using JWT Token Authentication  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(ApiVersion version, [FromQuery] string login, string senha)
        {
            IActionResult response = Unauthorized();
            var exists = await AuthenticateUser(login, senha);
            if (exists)
            {
                var tokenString = GenerateJSONWebToken(login, senha);
                response = Ok(new { Token = tokenString, Message = "Success" });
            }
            return response;
        }
        #endregion

        #region Get  
        /// <summary>  
        /// Authorize the Method  
        /// </summary>  
        /// <returns></returns>  
        [HttpGet(nameof(Get))]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return new string[] { accessToken };
        }


        #endregion

    }

}