// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;

namespace Mediapipe.Net.Calculators;

public abstract class CpuCalculator<TPacket, T> : Calculator<TPacket, T> where TPacket : Packet<T>, new()
{
    private readonly OutputStreamPoller<ImageFrame> framePoller;

    protected CpuCalculator(string graphPath, string? secondaryOutputStream = null)
        : base(graphPath, secondaryOutputStream)
    {
        framePoller = Graph.AddOutputStreamPoller<ImageFrame>("output_video").Value();
    }

    protected override ImageFrame SendFrame(ImageFrame frame)
    {
        using ImageFramePacket packet = new ImageFramePacket(frame, new Timestamp(base.CurrentFrame));
        Graph.AddPacketToInputStream("input_video", packet).AssertOk();
        ImageFramePacket imageFramePacket = new ImageFramePacket();
        framePoller.Next(imageFramePacket);
        return imageFramePacket.Get();
    }

    protected override void DisposeManaged()
    {
        base.DisposeManaged();
        framePoller.Dispose();
    }
}
