using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Abstract;

namespace Common.Constants
{
    public static class EntityConsts
    {
        public const string DataRow = "DataRow";

        public static string CorrectTableName<T>() where T : Entity => CorrectTableName(typeof(T));

        public static string CorrectTableName(Type type) => type.Name.Replace("DataRow", "");

        public static string CorrectLinkTableName(string linkBaseName)
        {
            return linkBaseName.EndsWith("Link", StringComparison.OrdinalIgnoreCase)
                ? linkBaseName
                : $"{linkBaseName}Link";
        }
    }
}
