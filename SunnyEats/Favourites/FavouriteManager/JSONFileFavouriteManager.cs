using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyEats.Favourites.FavouriteManager
{
    class JSONFileFavouriteManager : IFavouriteManager
    {
        /// <summary>
        /// Reads favourite file from external source then returns it as a class
        /// </summary>
        /// <param name="path">Can either be a file name, or path. This is the expected location of the file</param>
        /// <exception cref="IOException">Occurs when the File cannot be read</exception>
        /// <returns>the contents retrieved from the file</returns>
        public Favourite ReadFile(string path)
        {
            path += ".json";
            Favourite favourite = null;
            using (StreamReader reader = new StreamReader(File.Open(path, FileMode.Open)))
            {
               favourite = JsonConvert.DeserializeObject<Favourite>(reader.ReadToEnd());
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
            path += ".json";
            using (StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create)))
            {
                var favouriteJson = JsonConvert.SerializeObject(favourite);
                writer.Write(favouriteJson);
            }
        }
    }
}
