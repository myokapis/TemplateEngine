/* ****************************************************************************
Copyright 2018-2022 Gene Graves

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using ExampleWebSite.Models;

namespace ExampleWebSite.Services
{

    /// <summary>
    /// An interface for a simple data service
    /// </summary>
    public interface IDataService
    {
        IEnumerable<Item> GetItems();
        IEnumerable<Item> GetItems(Func<Item, bool> filter);
        IEnumerable<T> GetItemTypes<T>(Func<ItemType, T> mappingFunc);
        T GetPageScope<T>(Func<ItemType?, T> mappingFunc);
    }

    /// <summary>
    /// A simple data service to provide data in the example web site
    /// </summary>
    public class DataService : IDataService
    {

        /// <summary>
        /// Provides a collection of items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Item> GetItems()
        {
            return items;
        }

        /// <summary>
        /// Provides a filtered collection of items
        /// </summary>
        /// <param name="filter">A function for selecting items that match the desired filter</param>
        /// <returns></returns>
        public IEnumerable<Item> GetItems(Func<Item, bool> filter)
        {
            return GetItems().Where(a => filter(a));
        }

        /// <summary>
        /// Provides a collection of objects based on the entries in the ItemType enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mappingFunc">A function to map an enum entry to a class</param>
        /// <returns>A collection of items mapped from the ItemType enum</returns>
        public IEnumerable<T> GetItemTypes<T>(Func<ItemType, T> mappingFunc)
        {
            return (Enum.GetValues(typeof(ItemType)) as ItemType[])
                .Select(v => mappingFunc.Invoke(v));
        }

        /// <summary>
        /// A dummy helper method to provide a default page scope
        /// </summary>
        /// <typeparam name="T">The object type containing the page scope</typeparam>
        /// <param name="mappingFunc">A function for mapping the provided object type to a page scope object</param>
        /// <returns>An object of type T representing the page scope</returns>
        public T GetPageScope<T>(Func<ItemType?, T> mappingFunc)
        {
            // in a real website this method would setup and return the default page scope
            return mappingFunc.Invoke(null);
        }

        /// <summary>
        /// Hard-coded dummy data for demonstrating the site
        /// </summary>
        private static readonly Item[] items = new Item[]
        {
            new Item
            {
                Description = "A domesticated mammal of the Canidae family",
                FunFact = "Dogs were first domesticated around 10,000 BC",
                ImageUrl = "img/dog.jpg",
                Name = "Dog",
                Type = ItemType.Animal
            },
            new Item
            {
                Description = "A leafy vegetable of the Gemmifera group",
                FunFact = "Brussel sprouts were popular in Brussels, Belgium and received their name from that city",
                ImageUrl = "img/brussel-sprout.jpg",
                Name = "Brussel Sprout",
                Type = ItemType.Vegetable
            },
            new Item
            {
                Description = "A coarse-grained, banded metamorphic rock",
                FunFact = "Some of the oldest earth rocks are gneiss",
                ImageUrl = "img/gneiss.jpg",
                Name = "Gneiss",
                Type = ItemType.Mineral
            },
            new Item
            {
                Description = "An organic alcohol produced by the fermentation of sugars",
                FunFact = "Ethanol is considered a universal solvent",
                ImageUrl = "img/ethanol.jpg",
                Name = "Ethanol",
                Type = ItemType.Mineral
            },
            new Item
            {
                Description = "A domesticated mammal of the Mustelid family",
                FunFact = "Ferrets will perform a weasel war dance when they get excited",
                ImageUrl = "img/ferret.jpg",
                Name = "Ferret",
                Type = ItemType.Animal
            },
            new Item
            {
                Description = "A North American deciduous shrub",
                FunFact = "Witch hazel can produce flowers and fruit concurrently",
                ImageUrl = "img/witch-hazel.jpg",
                Name = "Witch Hazel",
                Type = ItemType.Vegetable
            },
            new Item
            {
                Description = "An igneous rock formed during lava flows",
                FunFact = "Obsidian is sometimes used in surgical scalpels",
                ImageUrl = "img/obsidian.jpg",
                Name = "Obsidian",
                Type = ItemType.Mineral
            },
            new Item
            {
                Description = "A plant of the Altissima group that stores sugar in its root",
                FunFact = "Napoleon opened schools to study the sugar beet",
                ImageUrl = "img/sugar-beet.jpg",
                Name = "Sugar Beet",
                Type = ItemType.Vegetable
            },
            new Item
            {
                Description = "A hard variety of coal containing a high carbon content",
                FunFact = "A vein of anthracite in Pennsylvania caught fire and has been burning since 1962",
                ImageUrl = "img/anthracite.jpg",
                Name = "Anthracite",
                Type = ItemType.Mineral
            },
            new Item
            {
                Description = "A perennial flowering bulb of the lily family",
                FunFact = "Tulips were first cultivated in Persia although today they are often associated with the Netherlands",
                ImageUrl = "img/tulip.jpg",
                Name = "Tulip",
                Type = ItemType.Vegetable
            },
            new Item
            {
                Description = "A colourful forest bird of the Trogon family",
                FunFact = "Quetzals are the national bird of Guatemala and also the name of its currency",
                ImageUrl = "img/quetzal.jpg",
                Name = "Quetzal",
                Type = ItemType.Animal
            }
        };

    }

}
