using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SunnyEats.Favourites;
using SunnyEats.Favourites.FavouriteManager;

namespace UnitTestSunnyEats
{
    [TestClass]
    public class UnitTestFavourite
    {
        /// <summary>
        /// Check that the BinaryFileFavouriteManager can both write and read valid Favourite data
        /// </summary>
        [TestMethod]
        public void TestMethodBinaryManager()
        {
            int[] ids = { 2, 4, 9 };
            var favourites = new Favourite();
            favourites.IDS = ids;

            // Write to file
            var binaryManager = new BinaryFileFavouriteManager();
            binaryManager.WriteFile("TestBinary", favourites);

            // Read from file
            favourites = null;
            favourites = binaryManager.ReadFile("TestBinary");

            // Check that favourite ids match
            Assert.AreEqual(ids.ToString(), favourites.IDS.ToString());
        }

        /// <summary>
        /// Check that the JSONFileFavouriteManager can both write and read valid Favourite data
        /// </summary>
        [TestMethod]
        public void TestMethodJSONManager()
        {
            int[] ids = { 3, 9, 2, 12, 5, 1 };
            var favourites = new Favourite();
            favourites.IDS = ids;

            // Write to file
            var jsonManager = new JSONFileFavouriteManager();
            jsonManager.WriteFile("TestJson", favourites);

            // Read from file
            favourites = null;
            favourites = jsonManager.ReadFile("TestJson");

            // Check that favourite ids match
            Assert.AreEqual(ids.ToString(), favourites.IDS.ToString());
        }

        /// <summary>
        /// Check that the XMLFileFavouriteManager can both write and read valid Favourite data
        /// </summary>
        [TestMethod]
        public void TestMethodXMLManager()
        {
            int[] ids = { 1, 2, 1, 9, 100, 120, 20, 3 };
            var favourites = new Favourite();
            favourites.IDS = ids;

            // Write to file
            var xmlManager = new XMLFileFavouriteManager();
            xmlManager.WriteFile("TestXml", favourites);

            // Read from file
            favourites = null;
            favourites = xmlManager.ReadFile("TestXml");

            // Check that favourite ids match
            Assert.AreEqual(ids.ToString(), favourites.IDS.ToString());
        }
    }
}
