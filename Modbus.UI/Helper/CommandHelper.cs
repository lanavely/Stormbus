using System.Collections.Generic;
using System.Linq;

namespace Stormbus.UI.Helper
{
    public static class CommandHelper
    {
        public static List<ResultItemModel> GenerateSignalModelList<T>(int startAddress, int count)
        {
            return Enumerable.Range(startAddress, count)
                .Select(a => new ResultItemModel {Address = (ushort) a, Value = default(T)}).ToList();
        }
        
        public static void UpdateItemsByCountChanged<T>(List<ResultItemModel> items, ushort newCount, ushort oldCount, ushort address)
        {
            if (newCount > oldCount)
            {
                items.RemoveRange(0, newCount - oldCount);
                items.AddRange(CommandHelper.GenerateSignalModelList<T>(address + oldCount, newCount - oldCount));
            }

            if (newCount < oldCount)
            {
                items.RemoveRange(items.Count-(oldCount + newCount), oldCount - newCount);
            }
        }

        public static void UpdateItemsByAddressChanged(List<ResultItemModel> items, ushort newAddress, ushort oldAddress)
        {
            foreach (var item in items)
            {
                item.Address = newAddress;
                newAddress++;
            }
        }
    }
}