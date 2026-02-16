using System;

namespace TicketPlatform.Models
{
    public enum UserRole
    {
        Unknown = 0,
        SalesRep,
        SVP,
        ITManager
    }

    public static class RolePrefixHelper
    {
        public static UserRole ParseRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return UserRole.Unknown;

            switch (role)
            {
                case "Sales Rep":
                    return UserRole.SalesRep;
                case "SVP":
                    return UserRole.SVP;
                case "IT Manager":
                    return UserRole.ITManager;
                default:
                    return UserRole.Unknown;
            }
        }

        public static string GetPrefix(UserRole role)
        {
            switch (role)
            {
                case UserRole.SalesRep:
                    return "W";
                case UserRole.SVP:
                    return "X";
                case UserRole.ITManager:
                    return "Y";
                default:
                    return null;
            }
        }
    }
}
