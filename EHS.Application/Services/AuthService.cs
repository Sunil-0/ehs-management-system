using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using BCrypt.Net;

namespace EHS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null)
                return null; // don't reveal whether the email exists -- generic failure

            // BCrypt.Verify re-hashes the plaintext password with the same salt
            // embedded in the stored hash and compares them -- the plaintext
            // password itself is never stored anywhere.
            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordValid)
                return null;

            var token = _tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Role = user.Role.ToString()
            };
        }
    }
}