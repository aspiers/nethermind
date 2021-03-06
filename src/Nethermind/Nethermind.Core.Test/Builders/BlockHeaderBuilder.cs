﻿/*
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

using System.Numerics;
using Nethermind.Core.Crypto;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Core.Test.Builders
{
    public class BlockHeaderBuilder : BuilderBase<BlockHeader>
    {
        public static UInt256 DefaultDifficulty = 1_000_000;
        
        public BlockHeaderBuilder()
        {
            TestObjectInternal = new BlockHeader(
                                     Keccak.Compute("parent"),
                                     Keccak.OfAnEmptySequenceRlp,
                                     Address.Zero,
                                     DefaultDifficulty, 0,
                                     4_000_000,
                                     1_000_000,
                                     new byte[] {1, 2, 3});
            TestObjectInternal.Bloom = new Bloom();
            TestObjectInternal.MixHash = Keccak.Compute("mix_hash");
            TestObjectInternal.Nonce = 1000;
            TestObjectInternal.ReceiptsRoot = Keccak.EmptyTreeHash;
            TestObjectInternal.StateRoot = Keccak.EmptyTreeHash;
            TestObjectInternal.TransactionsRoot = Keccak.EmptyTreeHash;
            TestObjectInternal.Hash = BlockHeader.CalculateHash(TestObjectInternal);
        }
        
        public BlockHeaderBuilder WithParent(BlockHeader parentHeader)
        {
            TestObjectInternal.ParentHash = parentHeader.Hash;
            TestObjectInternal.Number = parentHeader.Number + 1;
            TestObjectInternal.GasLimit = parentHeader.GasLimit;
            return this;
        }
    }
}