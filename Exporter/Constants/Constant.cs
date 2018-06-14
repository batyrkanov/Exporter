using System.Collections.Generic;

namespace Exporter.Constants
{
    public static class Constant
    {
        public static Dictionary<int, string> InputTypes
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    { 1, "text" },
                    { 2, "number" },
                    { 3, "date" },
                    { 4, "time" },
                    { 5, "week" },
                    { 6, "month" },
                    { 7, "email" },
                    { 8, "tel" }
                };
            }
        }

        public static int NumberOfQueriesPerPage
        {
            get { return 10; }
        }
    }
}