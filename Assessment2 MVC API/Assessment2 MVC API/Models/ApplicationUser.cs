using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;

namespace Assessment2_MVC_API.Models // directly from: https://youtu.be/fKarVGX2Vl8 //https://youtu.be/2R4RW7WaIWQ <-- this one
{
    [CollectionName("users")]
    public class ApplicationUser :  MongoIdentityUser<Guid>
    {

    }
}
