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
        /// <summary>
        /// Reads favourite file from external source then returns it as a class
        /// </summary>
        /// <param name="path">Can either be a file name, or path. This is the expected location of the file</param>
        /// <exception cref="IOException">Occurs when the File cannot be read</exception>
        /// <returns>the contents retrieved from the file</returns>
        public Favourite ReadFile(string path)
        {
            Favourite favourite = null;
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                favourite = formatter.Deserialize(stream) as Favourite;
            }

            return favourite;
        }

        /// <summary>
        /// Writes favourite file to an external source
        /// </summary>
        /// <param name="path">Can either be a file name, or path. This is the location to save the file to.</param>
        /// <exception cref="IOException">Occurs when the File cannot be written</exception>
        /// <returns>the contents retrieved from the file</returns>
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
