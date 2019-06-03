using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SunnyEats.Favourites.FavouriteManager
{
    class BinaryFileFavouriteManager : IFavouriteManager
    {
        public Favourite ReadFile(string fileName)
        {
            Favourite favourite = null;
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                favourite = formatter.Deserialize(stream) as Favourite;
            }

            return favourite;
        }

        public void WriteFile(string path, Favourite favourite)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, favourite);
            }
        }
    }
}
