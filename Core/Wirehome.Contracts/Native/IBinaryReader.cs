﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wirehome.Contracts.Core
{
    public interface IBinaryReader : IDisposable
    {
        byte ReadByte();
        string ReadString(byte size);
        float ReadSingle();
        uint ReadUInt32();
        Task<uint> LoadAsync(uint count, CancellationToken cancellationToken);
    }
}