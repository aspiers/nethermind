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
using System.Collections.Concurrent;
using System.Linq;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Encoding;
using Nethermind.Core.Specs;
using Nethermind.Dirichlet.Numerics;
using Nethermind.Store;

namespace Nethermind.Blockchain
{
    public class TransactionStore : ITransactionStore
    {
        private readonly ConcurrentDictionary<Keccak, Transaction> _pending = new ConcurrentDictionary<Keccak, Transaction>();
        private readonly IDb _receiptsDb;
        private readonly IEthereumSigner _signer;
        private readonly ISpecProvider _specProvider;

        public TransactionStore(IDb receiptsDb, ISpecProvider specProvider, IEthereumSigner signer)
        {
            _specProvider = specProvider ?? throw new ArgumentNullException(nameof(specProvider));
            _receiptsDb = receiptsDb ?? throw new ArgumentNullException(nameof(receiptsDb));
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public void StoreProcessedTransaction(Keccak txHash, TransactionReceipt receipt)
        {
            if (receipt == null) throw new ArgumentNullException(nameof(receipt));

            IReleaseSpec spec = _specProvider.GetSpec(receipt.BlockNumber);
            _receiptsDb.Set(txHash, Rlp.Encode(receipt, spec.IsEip658Enabled ? RlpBehaviors.Eip658Receipts | RlpBehaviors.Storage : RlpBehaviors.Storage).Bytes);
        }

        public TransactionReceipt GetReceipt(Keccak txHash)
        {
            var receiptData = _receiptsDb.Get(txHash);
            if (receiptData == null) return null;

            Rlp rlp = new Rlp(receiptData);
            return Rlp.Decode<TransactionReceipt>(rlp, RlpBehaviors.Storage);
        }

        public AddTransactionResult AddPending(Transaction transaction, UInt256 blockNumber)
        {
            if (_pending.ContainsKey(transaction.Hash))
            {
                NewPending?.Invoke(this, new TransactionEventArgs(transaction)); // hack
                return AddTransactionResult.AlreadyKnown;
            }

            Address recoveredAddress = _signer.RecoverAddress(transaction, blockNumber);
            if (recoveredAddress != transaction.SenderAddress)
            {
                throw new InvalidOperationException("Invalid signature");
            }

            _pending[transaction.Hash] = transaction;
            NewPending?.Invoke(this, new TransactionEventArgs(transaction));
            return AddTransactionResult.Added;
        }

        public void RemovePending(Transaction transaction)
        {
            if (_pending.ContainsKey(transaction.Hash)) _pending.TryRemove(transaction.Hash, out Transaction _);
        }

        public Transaction[] GetAllPending()
        {
            var result = _pending.Values.ToArray();
            return result;
        }

        public event EventHandler<TransactionEventArgs> NewPending;
    }
}