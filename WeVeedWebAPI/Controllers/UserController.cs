using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeVeed.Application.Services;
using WeVeed.Application.Dtos;
using Resources.Base.Responses;
using WeVeedWebAPI.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Resources.Base.SettingsModels;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using Resources.Base.AuthUtils;
using System.Linq;
using Resources.Base.Exception;
using System.Net.Http;
using WeVeedWebAPI.Utils;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserAppService _userAppService;
        private readonly IOptions<AppGeneralSettings> _appGeneralSettings;
        private static readonly HttpClient client = new HttpClient();

        // Login With Facebook important 
        private static readonly string fbGraphApiUrl = "https://graph.facebook.com/debug_token";
        private static readonly string fbAccessToken = "1817147274990321|4VPSIjQIo-iUsimRoXhMOSH3yBA"; // the App Id 
        private static readonly string fbAppId = "1817147274990321"; // the App Id 

        private static readonly List<string> adminsEmails = new List<string>
        {
            "stiuca.mihai@yahoo.com",
            "cristian.cirstocea@yahoo.ro",
            "cristian.cirstocea7@gmail.com",
            "cristina.alexandru21@yahoo.com",
            "office@weveed.com",
        };

        public UserController(IUserAppService userAppService, IOptions<AppGeneralSettings> appGeneralSettings)
        {
            _userAppService = userAppService;
            _appGeneralSettings = appGeneralSettings;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> Get(string id)
        {
            var a = Request.Headers;
            var x = User.Claims;
            return new string[] { "111111111", "22222" };
        }

        //[HttpGet("deleteNotValidated")]
        //public async Task<IActionResult> DeleteNotValidated()
        //{
        //    await _userAppService.DeleteNotValidated();
        //    return Response.Ok(new BaseResponse(true));
        //}

        #region Reset Password

        // send an email to the email address to change the password 
        [HttpPost("resetPasswordSendEmail")]
        public async Task<IActionResult> ResetPasswordSendEmail([FromBody] ResetPasswordSendEmailInput input)
        {
            var wasEmailSent = await _userAppService.SendResetPasswordEmailAsync(input);
            return Response.Ok(new BaseResponse(wasEmailSent));
        }

        // reset the password
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var wasPasswordReset = await _userAppService.ResetPasswordAsync(input);
            return Response.Ok(new BaseResponse(wasPasswordReset));
        }

        #endregion

        [HttpPost("validateProducerByAdmin")]
        public async Task<IActionResult> ValidateProducerByAdmin([FromBody] ValidateProducerByAdminInput input)
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var currentUserEntity = await _userAppService.GetBasicInfoById(currentUserId);

            if (!adminsEmails.Contains(currentUserEntity.Email.ToLower()))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var validationResult = await _userAppService.ValidateProducerByAdmin(input);

            return Response.Ok(new BaseResponse(true));
        }

        [HttpPost("searchProducers")]
        public async Task<IActionResult> SearchProducers([FromBody] SearchProducerInput input)
        {
            var producers = await _userAppService.SearchProducerAsync(input.Word);
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducers")]
        public async Task<IActionResult> GetDiscoverProducers()
        {
            var producers = await _userAppService.GetDiscoverProducersAsync();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersFollowedWeekly")]
        public async Task<IActionResult> GetDiscoverProducersFollowedWeekly()
        {
            var producers = await _userAppService.GetDiscoverProducersFollowedWeekly();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersFollowedMonthly")]
        public async Task<IActionResult> GetDiscoverProducersFollowedMonthly()
        {
            var producers = await _userAppService.GetDiscoverProducersFollowedMonthly();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersFollowedAllTime")]
        public async Task<IActionResult> GetDiscoverProducersFollowedAllTime()
        {
            var producers = await _userAppService.GetDiscoverProducersFollowedAllTime();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersMostRecent")]
        public async Task<IActionResult> GetDiscoverProducersMostRecent()
        {
            var producers = await _userAppService.GetDiscoverMostRecentProducers();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersMostViewedWeekly")]
        public async Task<IActionResult> GetDiscoverProducersMostViewedWeekly()
        {
            var producers = await _userAppService.GetDiscoverProducersMostViewedWeekly();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersMostViewedMonthly")]
        public async Task<IActionResult> GetDiscoverProducersMostViewedMonthly()
        {
            var producers = await _userAppService.GetDiscoverProducersMostViewedMonthly();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverProducersMostViewedAllTime")]
        public async Task<IActionResult> GetDiscoverProducersMostViewedAllTime()
        {
            var producers = await _userAppService.GetDiscoverProducersMostViewedAllTime();
            var response = new BaseResponse<List<ProducerListViewDto>>(producers);
            return Response.Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterInput input)
        {
            var userRegistered = await _userAppService.RegisterAsync(input);
            var response = new BaseResponse(userRegistered);
            return Response.Ok(response);
        }

        [HttpPost("registerWithFacebook")]
        public async Task<IActionResult> RegisterWithFacebook([FromBody] UserFBRegisterInput input)
        {
            return Response.Ok(new BaseResponse(true));
            var verifyTokenResult = await VerifyFBToken(input.FBToken, input.FacebookUserId);
            if (!verifyTokenResult)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var alreadyCreatedUser = await _userAppService.GetBasicInfoByFacebookId(input.FacebookUserId);
            // user already exists 
            if(alreadyCreatedUser != null)
            {
                return Response.ServerError(new BaseResponse(false));
            }

            var userCreated = await _userAppService.RegisterWithFBAsync(input);

            if (userCreated == null)
            {
                return Response.ServerError(new BaseResponse(false));
            }

            // AFTER CREATING THE FACEBOOK USER, IMMEDIATELLY LOG HIM IN 
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, userCreated.Id),
                    new Claim(AppClaims.UserType, userCreated.UserType)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            userCreated.Token = newTokenString;

            return Response.Ok(new BaseResponse<UserBasicInfoDto>(userCreated));
        }

        [HttpPost("confirmAccount")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountInput input)
        {
            var userConfirmed = await _userAppService.ConfirmAccountAsync(input.Code);
            var response = new BaseResponse(userConfirmed);
            return Response.Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginInput input)
        {
            var authenticationResult = await _userAppService.AuthenticateAsync(input);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, authenticationResult.Id),
                    new Claim(AppClaims.UserType, authenticationResult.UserType)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            authenticationResult.Token = newTokenString;

            var response = new BaseResponse<UserBasicInfoDto>(authenticationResult);

            return Response.Ok(response);
        }

        // DOES THE SAME THING AS METHOD VerifyRegisterWithFacebook
        [HttpPost("loginWithFacebook")]
        public async Task<IActionResult> LoginWithFacebook([FromBody] UserLoginWithFBInput input)
        {
            return Response.Ok(new BaseResponse(true));
            if (string.IsNullOrWhiteSpace(input.UserId) || string.IsNullOrWhiteSpace(input.FBToken))
            {
                return Response.ServerError(new BaseResponse(false));
            }

            var user = await _userAppService.GetBasicInfoByFacebookId(input.UserId);

            var verifyTokenResult = await VerifyFBToken(input.FBToken, input.UserId);
            if (!verifyTokenResult)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var response = new UserVerifyFacebookRegisterDto();

            if (user == null)
            {
                response.DoesUserAlreadyExist = false;
                response.LoggedUser = null;
                return Response.Ok(new BaseResponse<UserVerifyFacebookRegisterDto>(response));
            }

            // IF THE USER ALREADY EXISTS, LOG HIM IN 
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, user.Id),
                    new Claim(AppClaims.UserType, user.UserType)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            user.Token = newTokenString;
            response.DoesUserAlreadyExist = true;
            response.LoggedUser = user;

            return Response.Ok(new BaseResponse<UserVerifyFacebookRegisterDto>(response));
        }

        [HttpPost("verifyRegisterWithFacebook")]
        public async Task<IActionResult> VerifyRegisterWithFacebook([FromBody] UserVerifyRegisterFacebookInput input)
        {
            return Response.Ok(new BaseResponse(true));
            if (string.IsNullOrWhiteSpace(input.UserId) || string.IsNullOrWhiteSpace(input.FBToken))
            {
                return Response.ServerError(new BaseResponse(false));
            }

            var user = await _userAppService.GetBasicInfoByFacebookId(input.UserId);

            var verifyTokenResult = await VerifyFBToken(input.FBToken, input.UserId);
            if (!verifyTokenResult)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var response = new UserVerifyFacebookRegisterDto();

            if(user == null)
            {
                response.DoesUserAlreadyExist = false;
                response.LoggedUser = null;
                return Response.Ok(new BaseResponse<UserVerifyFacebookRegisterDto>(response));
            }

            // IF THE USER ALREADY EXISTS, LOG HIM IN 
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, user.Id),
                    new Claim(AppClaims.UserType, user.UserType)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            user.Token = newTokenString;
            response.DoesUserAlreadyExist = true;
            response.LoggedUser = user;

            return Response.Ok(new BaseResponse<UserVerifyFacebookRegisterDto>(response));
        }

        private async Task<bool> VerifyFBToken(string token, string fbUserId)
        {
            return false;
            try
            {
                var url = $"{fbGraphApiUrl}?input_token={token}&access_token={fbAccessToken}";
                var response = await client.GetStringAsync(url);

                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookCheckResponse>(response);

                if (!responseObj.Data.Is_Valid)
                {
                    return false;
                }

                if(responseObj.Data.App_Id != fbAppId)
                {
                    return false;
                }

                if(responseObj.Data.User_Id != fbUserId)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("isEmailUnique")]
        public async Task<IActionResult> IsEmailUnique([FromBody] IsEmailUniqueInput input)
        {
            return Response.Ok(new BaseResponse(true));
        }

        [Authorize]
        [HttpPost("isLoggedProducerNameUnique")]
        public async Task<IActionResult> IsLoggedProducerNameUnique([FromBody] IsLoggedProducerNameUniqueInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }
            
            var isUnique = await _userAppService.IsProducerNameUnique(input.ProducerName, id);
            if (isUnique)
            {
                return Response.Ok(new BaseResponse(true));
            }
            else
            {
                var response = new BaseResponse(new List<PropertyError>
                {
                    new PropertyError { PropertyName = "ProducerName", Errors = new List<string> { "Numele de producator este deja folosit." } }
                }, 422);
                return Response.ValidationError(response);
            }
        }

        [HttpPost("isProducerNameUnique")]
        public async Task<IActionResult> IsProducerNameUnique([FromBody] IsProducerNameUniqueInput input)
        {
            return Response.Ok(new BaseResponse(true));
        }

        [Authorize]
        [HttpGet("getUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if(id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            var user = await _userAppService.GetBasicInfoById(id);
            return Response.Ok(new BaseResponse<UserBasicInfoDto>(user));
        }

        [HttpGet("test")]
        public IActionResult GetT()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserUI, randomString)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            var response = new BaseResponse<string>(newTokenString);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("updateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UserUpdateInfoInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            var result = await _userAppService.UpdateUserInfoAsync(id, input);
            return Response.Ok(new BaseResponse(result));
        }

        [Authorize]
        [HttpPost("updateProducerInfo")]
        public async Task<IActionResult> UpdateProducerInfo([FromBody] ProducerUpdateInfoInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            var result = await _userAppService.UpdateProducerInfoAsync(id, input);
            return Response.Ok(new BaseResponse(result));
        }

        [Authorize]
        [HttpPost("becomeProducer")]
        public async Task<IActionResult> BecomeProducer([FromBody] UserBecomeProducerInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            var result = await _userAppService.UserBecomeProducerAsync(id, input);

            if (!result)
            {
                return Response.ServerError(new BaseResponse<UserBecomeProducerDto>(null));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, id),
                    new Claim(AppClaims.UserType, "producer")
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            return Response.Ok(new BaseResponse<UserBecomeProducerDto>(new UserBecomeProducerDto() { Token = newTokenString }));
        }

        #region For Producer's Page 

        [HttpGet("getProducerViewInfo/{producerId}")]
        public async Task<IActionResult> GetProducerViewInfo(string producerId)
        {
            if (string.IsNullOrWhiteSpace(producerId))
            {
                throw new HttpStatusCodeException(404, new List<string> { "Id-ul producatorului este necesar." });
            }

            var result = await _userAppService.GetProducerViewDtoAsync(producerId);
            return Response.Ok(new BaseResponse<ProducerViewDto>(result));
        }

        #endregion
    }
}
