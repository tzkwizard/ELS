using FireSharp.Interfaces;

namespace LMS.Common.DAL
{
    public interface IFBoperation
    {
        IFirebaseClient GetFirebaseClient();
        IFirebaseClient GetFirebaseClient(string node);       
        string GetFirebaseToken(string user, string uid, string data);       
    }
}