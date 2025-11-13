using PRAKTIKA4_KYRS_DEMO.BD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAKTIKA4_KYRS_DEMO
{
    public static class Session
    {
        public static User CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;

            public static void Logout()
            {
                CurrentUser = null;
            }
    }
}
