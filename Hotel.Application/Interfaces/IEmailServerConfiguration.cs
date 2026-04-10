using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface IEmailServerConfiguration
    {
         string Endpoint { get; }
         string Server { get;  }
         string Method { get;  }
         int Port { get;  }
         string Username { get;  }
         string Password  { get;  } 
         string From { get;  }
          string MessageBody { get; }
        string MessageSubject { get; }
        string EmailServiceEnabled { get; }
        bool UseSsl { get; }
        string EmailBodyBeforeChangePassword { get; }
        string EmailSubjectBeforeChangePassword { get; }
        string EmailChangePassswordUrl { get; }
        string EmailConfirmUrl { get; }

    }
}