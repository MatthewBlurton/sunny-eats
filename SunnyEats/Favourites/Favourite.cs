using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SunnyEats.Favourites
{
    [Serializable()]
    public class Favourite : ISerializable
    {
        public Favourite() { }
        public Favourite(int[] id)
        {
            IDS = id;
        }
        public Favourite(SerializationInfo info, StreamingContext context)
        {
            IDS = info.GetValue("Ids", typeof(int[])) as int[];
        }

        private int[] _ids;

        public int[] IDS
        {
            get { return _ids; }
            set => _ids = value;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Ids", IDS);
        }
    }
}
