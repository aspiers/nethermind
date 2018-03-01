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

using System;
using Nevermind.Core;
using Nevermind.Core.Crypto;
using Nevermind.Core.Extensions;
using Nevermind.Core.Potocol;
using Nevermind.Network.Rlpx.Handshake;
using NUnit.Framework;

namespace Nevermind.Network.Test.Rlpx.Handshake
{
    [TestFixture]
    public class AuthEip8MessageSerializerTests
    {
        private const string TestPrivateKeyHex = "0x3a1076bf45ab87712ad64ccb3b10217737f7faacbf2872e88fdd9a537d8fe266";

        private readonly Random _random = new Random(1);

        private readonly PrivateKey _privateKey = new PrivateKey(TestPrivateKeyHex);

        private readonly AuthEip8MessageSerializer _serializer = new AuthEip8MessageSerializer();

        private void TestEncodeDecode(IEthereumSigner signer)
        {
            AuthEip8Message authMessage = new AuthEip8Message();
            authMessage.Nonce = new byte[AuthMessageSerializer.NonceLength]; // sic!
            authMessage.Signature = signer.Sign(_privateKey, Keccak.Compute("anything"));
            authMessage.PublicKey = _privateKey.PublicKey;
            _random.NextBytes(authMessage.Nonce);
            byte[] data = _serializer.Serialize(authMessage);
            AuthEip8Message after = _serializer.Deserialize(data);

            Assert.AreEqual(authMessage.Signature, after.Signature);
            Assert.AreEqual(authMessage.PublicKey, after.PublicKey);
            Assert.True(Bytes.UnsafeCompare(authMessage.Nonce, after.Nonce));
            Assert.AreEqual(authMessage.Version, after.Version);
        }

        [TestCase(ChainId.MainNet)]
        [TestCase(ChainId.Morden)]
        [TestCase(ChainId.RootstockMainnet)]
        [TestCase(ChainId.DefaultGethPrivateChain)]
        [TestCase(ChainId.EthereumClassicMainnet)]
        [TestCase(ChainId.EthereumClassicTestnet)]
        public void Encode_decode_before_eip155(ChainId chainId)
        {
            EthereumSigner signer = new EthereumSigner(Frontier.Instance, chainId);
            TestEncodeDecode(signer);
        }

        [TestCase(ChainId.MainNet)]
        [TestCase(ChainId.Ropsten)]
        [TestCase(ChainId.Kovan)]
        public void Encode_decode_with_eip155(ChainId chainId)
        {
            EthereumSigner signer = new EthereumSigner(Byzantium.Instance, chainId);
            TestEncodeDecode(signer);
        }
    }
}