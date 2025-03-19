using System.Net.Http.Json;
using Microsoft.Extensions.Http; 
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Domain.Entities;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;
using UndefinedCRM.Infrastructure.Security.Tokens.Google;
using UndefinedCRM.Infrastructure.Security.Tokens.Access;

namespace UndefinedCRM.Application.UseCases.Users.GoogleAuth
{
    public class GoogleAuthUseCase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthUseCase(
            UserRepository userRepository, 
            JwtTokenGenerator tokenGenerator,
            IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseGoogleAuthJson> Execute(RequestGoogleAuthJson request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                throw new ErrorOnValidationException(new List<string> { "Google token is required" });
            }
            
            var googleTokenInfo = await ValidateGoogleToken(request.Token);
            if (googleTokenInfo == null)
            {
                throw new ErrorOnValidationException(new List<string> { "Invalid Google token" });
            }

            var user = await _userRepository.GetUserByEmailAsync(googleTokenInfo.Email);

            if (user == null)
            {
                var newUser = new User
                {
                    Name = googleTokenInfo.Name,
                    Email = googleTokenInfo.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                };

                var userId = await _userRepository.CreateUserAsync(newUser);
                newUser.Id = userId;
                user = newUser;
            }
            
            var token = _tokenGenerator.Generate(user);

            return new ResponseGoogleAuthJson
            {
                Email = user.Email,
                Name = user.Name,
                AccessToken = token
            };
        }

        private async Task<GoogleTokenInfo?> ValidateGoogleToken(string token)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={token}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();
            }
            catch
            {
                return null;
            }
        }
    }
}

