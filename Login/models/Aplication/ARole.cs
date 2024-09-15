using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Login.models.Aplication
{
    [CollectionName("role")]
    public class ARole : MongoIdentityRole<Guid>
    {

    }
}
