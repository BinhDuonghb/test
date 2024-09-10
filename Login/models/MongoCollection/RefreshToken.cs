using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Login.models.MongoCollection
{
    [CollectionName("RefreshToken")]
    
    public class RefreshToken
    {
        [BsonId]
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        
        [BsonElement("refreshToken")]
        public string RefreshTo{ get; set; } = string.Empty;
        [BsonElement("accessToken")]
        public string AccessToken { get; set; } =string.Empty;
    }
}
