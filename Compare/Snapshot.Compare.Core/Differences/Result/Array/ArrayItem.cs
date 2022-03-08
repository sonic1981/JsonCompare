using Snapshot.Contract.Compare;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snapshot.Compare.Core.Differences.Result.Array
{
    public class ArrayItem
    {
        public ArrayItem()
        {
            Lines = new List<SingleLineResult>();
        }
        public List<SingleLineResult> Lines { get; set; }

        public override int GetHashCode()
        {
            int hash = 1267;
            unchecked
            {
                foreach (SingleLineResult l in this.Lines)
                {
                    hash = hash * 181 + l.GetHashCode();
                }
            }

            return hash;
        }
    }
}
