// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Gpu;

namespace Mediapipe.Net.Calculators;

[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("Android")]
public abstract class GpuCalculator<TPacket, T> : Calculator<TPacket, T> where TPacket : Packet<T>, new()
{
    private readonly GpuResources gpuResources;

    private readonly GlCalculatorHelper gpuHelper;

    private readonly OutputStreamPoller<GpuBuffer> framePoller;

    protected GpuCalculator(string graphPath, string? secondaryOutputStream)
        : base(graphPath, secondaryOutputStream)
    {
        gpuResources = GpuResources.Create().Value();
        Graph.SetGpuResources(gpuResources);
        gpuHelper = new GlCalculatorHelper();
        gpuHelper.InitializeForTest(Graph.GetGpuResources());
        framePoller = Graph.AddOutputStreamPoller<GpuBuffer>("output_video").Value();
    }

    protected unsafe override ImageFrame SendFrame(ImageFrame frame)
    {
        //ImageFrame frame = frame;
        gpuHelper.RunInGlContext(delegate
        {
            GlTexture glTexture2 = gpuHelper.CreateSourceTexture(frame);
            GpuBuffer gpuBufferFrame = glTexture2.GetGpuBufferFrame();
            Gl.Flush();
            glTexture2.Release();
            GpuBufferPacket packet = new GpuBufferPacket(gpuBufferFrame.MpPtr);
            packet.At(new Timestamp(base.CurrentFrame));
            Graph.AddPacketToInputStream("input_video", packet);
            //return Status.Ok(isOwner: true);
        }).AssertOk();
        GpuBufferPacket outPacket = new GpuBufferPacket();
        framePoller.Next(outPacket);
        ImageFrame outFrame = null;
        gpuHelper.RunInGlContext(delegate
        {
            GpuBuffer gpuBuffer = outPacket.Get();
            GlTexture glTexture = gpuHelper.CreateSourceTexture(gpuBuffer);
            outFrame = new ImageFrame(gpuBuffer.Format.ImageFormatFor(), gpuBuffer.Width, gpuBuffer.Height, ImageFrame.GlDefaultAlignmentBoundary);
            gpuHelper.BindFramebuffer(glTexture);
            GlTextureInfo glTextureInfo = gpuBuffer.Format.GlTextureInfoFor(0);
            Gl.ReadPixels(0, 0, glTexture.Width, glTexture.Height, glTextureInfo.GlFormat, glTextureInfo.GlType, outFrame.MutablePixelData().ToPointer());
            Gl.Flush();
            glTexture.Release();
            //return Status.Ok(isOwner: true);
        }).AssertOk();
        if (outFrame == null)
        {
            throw new MediapipeNetException("!! FATAL - Frame is null on current GL context run!");
        }
        return outFrame;
    }

    protected override void DisposeManaged()
    {
        base.DisposeManaged();
        gpuResources.Dispose();
        gpuHelper.Dispose();
        framePoller.Dispose();
    }
}
