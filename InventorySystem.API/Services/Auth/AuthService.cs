using InventorySystem.API.DTOs.Auth;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventorySystem.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Domain validations using BusinessRuleException
            if(await _userRepository.ExistsByUsernameAsync(dto.Username))
            {
                throw new BusinessRuleException($"Username '{dto.Username}' is already taken.");
            }

            if (await _userRepository.ExistsByEmailAsync(dto.Email))
            {
                throw new BusinessRuleException($"Email '{dto.Email}' is already used by another account.");
            }

            // Instantiate and hash password with unique cryptographic salt

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Viewer" : dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            // Save the user via repository inheritance

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email, 
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Fetch and verify if user exists

            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if(user == null)
            {
                return null;
            }

            // verify hashed passwords 

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Mint signed JWT token

            var (token, expiresAt) = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = expiresAt,
            };
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var signingKey = new SymmetricSecurityKey(secretKey);
            var credidentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credidentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(tokenDescriptor);

            return (tokenString, expiresAt);
        }
    }
}
