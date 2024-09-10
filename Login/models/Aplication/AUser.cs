using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Login.models.Aplication
{
    [CollectionName("user")]
    public class AUser : MongoIdentityUser<Guid>
    {

    }
}
