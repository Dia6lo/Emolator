using System;
using System.Collections.Generic;
using System.Linq;

namespace Emolator
{
    public class DataBus
    {
        // TODO
        private List<Tuple<ushort, byte[]>> bindings = new List<Tuple<ushort, byte[]>>();

        public void Bind(ushort start, byte[] bytes)
        {
            bindings.Add(new Tuple<ushort, byte[]>(start, bytes));
        }

        public byte this[ushort address]
        {
            get
            {
                var memory = bindings.Where(b => b.Item1 <= address)
                    .OrderByDescending(b => b.Item1)
                    .First();
                return memory.Item2[address - memory.Item1];
            }
            set
            {
                var memory = bindings.Where(b => b.Item1 <= address)
                    .OrderByDescending(b => b.Item1)
                    .First();
                memory.Item2[address - memory.Item1] = value;
            }
        }
    }
}