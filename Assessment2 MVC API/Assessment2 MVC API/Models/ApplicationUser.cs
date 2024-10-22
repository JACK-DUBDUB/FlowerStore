using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;

namespace Assessment2_MVC_API.Models // directly from: https://youtu.be/fKarVGX2Vl8
{
    [CollectionName("Users")]
    public class ApplicationUser :  MongoIdentityUser<Guid>
    {

    }
}
