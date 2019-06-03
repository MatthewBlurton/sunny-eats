using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SunnyEats.Favourites
{
    [Serializable()]
    class Favourite : ISerializable
    {
        public Favourite(int[] id)
        {
            ID = id;
        }
        public Favourite(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetValue("Id", typeof(int[])) as int[];
        }

        private int[] _id;

        public int[] ID
        {
            get { return _id; }
            set => _id = value;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", ID);
        }
    }
}
