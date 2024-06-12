// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Calculators;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Native;
using System.Collections.Generic;
using System;
using System.Runtime.Versioning;

namespace Mediapipe.Net.Framework.Packets;

public class NormalizedLandmarkListVectorPacket : Packet<List<NormalizedLandmarkList>>
{
    public NormalizedLandmarkListVectorPacket()
    {
    }

    public NormalizedLandmarkListVectorPacket(IntPtr ptr, bool isOwner = true)
        : base(ptr, isOwner)
    {
    }

    public unsafe override List<NormalizedLandmarkList> Get()
    {
        UnsafeNativeMethods.mp_Packet__GetNormalizedLandmarkListVector(base.MpPtr, out var serializedProtoVector).Assert();
        GC.KeepAlive(this);
        List<NormalizedLandmarkList> result = serializedProtoVector.Deserialize<NormalizedLandmarkList>(NormalizedLandmarkList.Parser);
        serializedProtoVector.Dispose();
        return result;
    }

    public override StatusOr<List<NormalizedLandmarkList>> Consume()
    {
        throw new NotSupportedException();
    }

    public override Status ValidateAsType()
    {
        throw new NotImplementedException();
    }
}
