using Microsoft.IdentityModel.Tokens;
using DockScripter.Domain.Dtos.Requests;
using DockScripter.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;
using DockScripter.Repositories.Interfaces;

namespace DockScripter.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration,
            ITokenBlacklistService tokenBlacklistService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenBlacklistService = tokenBlacklistService;
            _logger = logger;
        }

        public async Task<bool> RegisterUser(RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            if (await _userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken))
                return false;

            CreatePasswordHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email!,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var requestRole = request.Role?.ToLower().Trim() ?? string.Empty;

            switch (requestRole)
            {
                case "owner":
                    user.Roles.Add(UserRole.Owner);
                    break;
                case "user":
                    user.Roles.Add(UserRole.User);
                    break;
                default:
                    throw new InvalidDataException($"Invalid role provided. Provided role: {requestRole}.");
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<string?> LoginUser(LoginUserRequestDto request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email,
                cancellationToken);
            if (user == null || !VerifyPasswordHash(request.Password!, user.PasswordHash!, user.PasswordSalt!))
                return null;

            return CreateToken(user);
        }

        public void LogoutUser(string token)
        {
            _tokenBlacklistService.BlacklistToken(token);
        }

        private string CreateToken(UserEntity userEntity)
        {
            var key = _configuration["Jwt:Key"];

            if (key == null)
            {
                _logger.LogError("JWT key is not configured.");
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userEntity.Id.ToString()),
                new Claim(ClaimTypes.Name, userEntity.Email!)
            };

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}