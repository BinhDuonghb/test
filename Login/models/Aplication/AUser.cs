using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Login.models.Aplication
{
    [CollectionName("user")]
    public class AUser : MongoIdentityUser<Guid>
    {
        public string StudentImageURL { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
