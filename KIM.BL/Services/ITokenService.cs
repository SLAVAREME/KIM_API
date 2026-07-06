using KIM.DAL.Entities;

namespace KIM.BL.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
