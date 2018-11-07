using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Services
{
    public interface IEmailClientSender
    {
        Task SendHelloWorldEmail(string email, string name);
    }
}
