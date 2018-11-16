using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreAPI
{
    public class UserDAO
    {
        private IConfiguration _configuration;

        public UserDAO(IConfiguration p_cnfg)
        {
            _configuration = p_cnfg;
        }

        public User Find(string p_userId)
        {
            string strUserId = "gabriel";
            string strAcessKey = "102030";

            if(p_userId == strUserId)
                return new User { UserID = strUserId,AccessKey = strAcessKey };

            return null;
        }
    }
}
