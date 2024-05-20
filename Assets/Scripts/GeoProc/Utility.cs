using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoProc
{
    class Utility
    {
        public static bool ContainsList<T>(List<List<T>> list, List<T> listItem)
        {
            listItem.Sort();

            for (int i = 0; i < list.Count; i++)
            {

                List<T> temp = list[i];

                if (temp.Count == listItem.Count)
                {
                    temp.Sort();

                    if (temp.SequenceEqual(listItem))
                    {
                        return true;
                    }

                }
            }

            return false;
        }
    }
}
