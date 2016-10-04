namespace Domain.Services.Abstract
{
    using Domain.Models;

    internal interface IUserService
    {
        User GetUser(string name, string password);
    }
}