using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyEats.Favourites
{
    interface IFavouriteManager
    {
        void WriteFile(string path, Favourite favourite);
        Favourite ReadFile(string path);
    }
}
