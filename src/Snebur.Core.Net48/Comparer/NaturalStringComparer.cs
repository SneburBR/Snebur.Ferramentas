using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snebur.Comparer
{
    public class NaturalStringComparer : IComparer<string>, IDisposable
    {
        private bool IsAscending { get; set; }

        private Dictionary<string, List<string>> Table { get; set; }

        private StringComparer Comparer { get; set; }

        public NaturalStringComparer(bool ascendingOrder = true)
        {
            this.Comparer = StringComparer.Create(new System.Globalization.CultureInfo("pt-BR"), true);
            this.Table = new Dictionary<string, List<string>>();
            this.IsAscending = ascendingOrder;
        }

        public int Compare(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }
            var x1 = new List<string>();
            var y1 = new List<string>();

            if (!this.Table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(".", " ").Replace("-", " ").Replace("_", " ").Replace("  ", " "), "([0-9]+)").ToList();
                this.Table.Add(x, x1);
            }
            if (!this.Table.TryGetValue(y, out y1))
            {
                y1 = Regex.Split(y.Replace(".", " ").Replace("-", " ").Replace("_", " ").Replace("  ", " "), "([0-9]+)").ToList();
                this.Table.Add(y, y1);
            }
            int returnVal = 0;

            int i = 0;
            while (i < x1.Count && i < y1.Count)
            {
                if (x1[i] != y1[i])
                {
                    returnVal = this.PartCompare(x1[i], y1[i]);
                    return this.IsAscending ? returnVal : -returnVal;
                }
                i += 1;
            }
            if (y1.Count > x1.Count)
            {
                returnVal = -1;
            }
            else if (x1.Count > y1.Count)
            {
                returnVal = 1;
            }
            else
            {
                returnVal = 0;
            }
            return this.IsAscending ? returnVal : -returnVal;
        }

        private int PartCompare(string left, string right)
        {
            int x = 0;
            int y = 0;
            if (!int.TryParse(left, out x))
            {
                return this.Comparer.Compare(left, right);
            }
            if (!int.TryParse(right, out y))
            {
                return this.Comparer.Compare(left, right);
            }
            return x.CompareTo(y);
        }
        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Table.Clear();
                    this.Table = null;
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}