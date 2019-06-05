using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SunnyEats.Favourites.FavouriteManager
{
    class XMLFileFavouriteManager : IFavouriteManager
    {
        /// <summary>
        /// Reads favourite file from external source then returns it as a class
        /// </summary>
        /// <param name="path">Can either be a file name, or path. This is the expected location of the file</param>
        /// <exception cref="IOException">Occurs when the File cannot be read</exception>
        /// <returns>the contents retrieved from the file</returns>
        public Favourite ReadFile(string path)
        {
            path += ".xml";
            Favourite favourite = null;
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var serialiser = new XmlSerializer(typeof(Favourite));
                favourite = serialiser.Deserialize(stream) as Favourite;
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
            // Add extension to path
            path += ".xml";
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var serialiser = new XmlSerializer(typeof(Favourite));
                serialiser.Serialize(stream, favourite);
            }
        }
    }
}
