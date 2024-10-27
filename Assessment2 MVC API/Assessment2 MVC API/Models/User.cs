using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Assessment2_MVC_API.Models
{
    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public int? Id { get; set; }

        [Required]
        [BsonElement("username"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [BsonElement("email"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [BsonElement("password"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Password { get; set; } = string.Empty;




        /*
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }   
        [Required]
        public string Password { get; set; }
        */
    }
}
