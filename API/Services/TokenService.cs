using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {

        /*
            The job of Microsoft.IdentityModel.Tokens that we installed from Nuget manager is to manage the logic of the token
            it means... not got it yet :(
        */
        private readonly UserManager<AppUser> _userManager;

        //this key do leave the server
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            //this key do leave the server
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public async Task<string> CreateToken(AppUser user)
        {
            //Diside claim by key and value - for this example our token will check by name only
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };


            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //Credentials = אישורים
            //pass the secret key from the configuration file (this key do leave the server)
            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);


            //Describe how our token will gonna look
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = cred
            };



            //Start to create the token and return it as string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}