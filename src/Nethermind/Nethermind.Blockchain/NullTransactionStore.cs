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
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Blockchain
{
    public class NullTransactionStore : ITransactionStore
    {
        private NullTransactionStore()
        {
        }

        public static NullTransactionStore Instance { get; } = new NullTransactionStore();

        public void StoreProcessedTransaction(Keccak txHash, TransactionReceipt receipt)
        {
        }

        public TransactionReceipt GetReceipt(Keccak txHash)
        {
            throw new NotSupportedException();
        }

        public AddTransactionResult AddPending(Transaction transaction, UInt256 blockNumber)
        {
            return AddTransactionResult.Added;
        }

        public void RemovePending(Transaction transaction)
        {
        }

        public Transaction[] GetAllPending()
        {
            return Array.Empty<Transaction>();
        }

        public event EventHandler<TransactionEventArgs> NewPending
        {
            add { }
            remove { }
        }
    }
}