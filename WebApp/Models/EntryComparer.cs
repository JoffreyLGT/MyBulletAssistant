using System.Collections.Generic;

namespace WebApp.Models
{
    public class EntryComparer : IComparer<Entry>
    {
        public int Compare(Entry x, Entry y)
        {
            // First compare their journals
            if (!x.Journal.Equals(y.Journal))
            {
                var result = x.Journal.CompareTo(y.Journal);
                switch (result)
                {
                    case 1:
                        return -1;
                    case -1:
                        return 1;
                    default:
                        return 0;
                }
            }

            // They are from the same journal so we compare the pages
            var xPage = GetStartingPage(x.Pages);
            var yPage = GetStartingPage(y.Pages);

            if (xPage.Length > yPage.Length)
            {
                return xPage.CompareTo(yPage.PadLeft(xPage.Length, '0'));
            }
            else if (xPage.Length < yPage.Length)
            {
                return xPage.PadLeft(yPage.Length, '0').CompareTo(yPage);
            }
            return xPage.CompareTo(yPage);
        }

        private string GetStartingPage(string page)
        {
            return page.Contains("-")
                ? page.Split('-')[0]
                : page.Contains(";")
                    ? page.Split(';')[0]
                    : page;
        }
    }
}
