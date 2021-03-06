/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Nethermind.Blockchain.Filters;
using NUnit.Framework;

namespace Nethermind.Blockchain.Test.Filters
{
    [TestFixture]
    public class FilterStoreTests
    {
        [Test]
        public void Can_save_and_load_block_filter()
        {
            FilterStore store = new FilterStore();
            BlockFilter filter = store.CreateBlockFilter(1);
            store.SaveFilter(filter);
            Assert.True(store.FilterExists(0), "exists");
            Assert.AreEqual(FilterType.BlockFilter, store.GetFilterType(filter.Id), "type");
        }

        [Test]
        public void Can_save_and_load_log_filter()
        {
            FilterStore store = new FilterStore();
            LogFilter filter = store.CreateLogFilter(new FilterBlock(1), new FilterBlock(2));
            store.SaveFilter(filter);
            Assert.True(store.FilterExists(0), "exists");
            Assert.AreEqual(FilterType.LogFilter, store.GetFilterType(filter.Id), "type");
        }

        [Test]
        public void Cannot_overwrite_filters()
        {
            FilterStore store = new FilterStore();

            BlockFilter externalFilter = new BlockFilter(100, 1);
            store.SaveFilter(externalFilter);
            Assert.Throws<InvalidOperationException>(() => store.SaveFilter(externalFilter));
        }

        [Test]
        public void Ids_are_incremented_when_storing_externally_created_filter()
        {
            FilterStore store = new FilterStore();

            BlockFilter externalFilter = new BlockFilter(100, 1);
            store.SaveFilter(externalFilter);
            LogFilter filter = store.CreateLogFilter(new FilterBlock(1), new FilterBlock(2));
            store.SaveFilter(filter);

            Assert.True(store.FilterExists(100), "exists 100");
            Assert.True(store.FilterExists(101), "exists 101");
            Assert.AreEqual(FilterType.LogFilter, store.GetFilterType(filter.Id), "type");
        }

        [Test]
        public void Remove_filter_removes_and_notifies()
        {
            FilterStore store = new FilterStore();
            BlockFilter filter = store.CreateBlockFilter(1);
            store.SaveFilter(filter);
            bool hasNotified = false;
            store.FilterRemoved += (s, e) => hasNotified = true;
            store.RemoveFilter(0);

            Assert.True(hasNotified, "notied");
            Assert.False(store.FilterExists(0), "exists");
        }
        
        [Test]
        public void Can_get_filters_by_type()
        {
            FilterStore store = new FilterStore();
            BlockFilter filter1 = store.CreateBlockFilter(1);
            store.SaveFilter(filter1);
            LogFilter filter2 = store.CreateLogFilter(new FilterBlock(1), new FilterBlock(2));
            store.SaveFilter(filter2);

            LogFilter[] logFilters = store.GetFilters<LogFilter>();
            BlockFilter[] blockFilters = store.GetFilters<BlockFilter>();
            
            Assert.AreEqual(1, logFilters.Length, "log filters length");
            Assert.AreEqual(1, logFilters[0].Id, "log filters ids");
            Assert.AreEqual(1, blockFilters.Length, "block Filters length");
            Assert.AreEqual(0, blockFilters[0].Id, "block filters ids");

        }
    }
}