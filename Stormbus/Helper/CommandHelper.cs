using System.Collections.ObjectModel;
using System.Linq;
using Stormbus.UI.Extenstion;

namespace Stormbus.UI.Helper
{
    public static class CommandHelper
    {
        public static ObservableCollection<ResultItemModel> GenerateSignalModelList<T>(int startAddress, int count)
        {
            return new ObservableCollection<ResultItemModel>(Enumerable.Range(startAddress, count)
                .Select(a => new ResultItemModel {Address = (ushort) a, Value = default(T)}).ToList());
        }
        
        public static void UpdateItemsByCountChanged<T>(ObservableCollection<ResultItemModel> items, ushort newCount, ushort oldCount, ushort address)
        {
            if (newCount > oldCount)
            {
                items.AddRange(GenerateSignalModelList<T>(address + oldCount, newCount - oldCount));
            }

            if (newCount < oldCount)
            {
                for (var i = items.Count; i > newCount; i--)
                    items.RemoveAt(i);
            }
        }

        public static void UpdateItemsByAddressChanged(ObservableCollection<ResultItemModel> items, ushort newAddress, ushort oldAddress)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                item.Address = newAddress;
                newAddress++;
            }
        }
    }
}