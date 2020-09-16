using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stormbus.UI.Extenstion
{
    public static class ObservableCollectionExt
    {
        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> rangeList)
        {
            foreach (var item in rangeList) observableCollection.Add(item);
        }
    }
}