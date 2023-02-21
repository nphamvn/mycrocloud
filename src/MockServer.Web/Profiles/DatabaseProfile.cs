using AutoMapper;
using MockServer.Core.Databases;
using MockServer.Web.Models.Database;

namespace MockServer.Web.Profiles;

public class DatabaseProfile: Profile
{
    public DatabaseProfile()
    {
        CreateMap<Database, DatabaseItem>();
        CreateMap<Database, SaveDatabaseViewModel>().ReverseMap();
    }
}
