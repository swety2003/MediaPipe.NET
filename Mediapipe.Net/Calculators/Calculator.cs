// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;

namespace Mediapipe.Net.Calculators;

public abstract class Calculator<TPacket, T> : Disposable where TPacket : Packet<T>, new()
{
    protected const string INPUT_VIDEO_STREAM = "input_video";

    protected const string OUTPUT_VIDEO_STREAM = "output_video";

    protected readonly string GraphPath;

    protected readonly string? SecondaryOutputStream;

    protected readonly CalculatorGraph Graph;

    private GCHandle? observeStreamHandle;

    public long CurrentFrame { get; private set; } = 0L;


    public event EventHandler<T>? OnResult;

    protected Calculator(string graphPath, string? secondaryOutputStream = null)
    {
        GraphPath = graphPath;
        SecondaryOutputStream = secondaryOutputStream;
        Graph = new CalculatorGraph(File.ReadAllText(GraphPath));
        if (SecondaryOutputStream == null)
        {
            return;
        }
        Graph.ObserveOutputStream<TPacket, T>(SecondaryOutputStream,packet=>
        {
            if (packet != null)
            {
                T e = packet.Get();
                this.OnResult?.Invoke(this, e);
            }
        }, out var callbackHandle).AssertOk();
        observeStreamHandle = callbackHandle;
    }

    public void Run()
    {
        Graph.StartRun().AssertOk();
    }

    protected abstract ImageFrame SendFrame(ImageFrame frame);

    public ImageFrame Send(ImageFrame frame, bool disposeSourceFrame = true)
    {
        lock (frame)
        {
            ImageFrame result = SendFrame(frame);
            CurrentFrame++;
            if (disposeSourceFrame)
            {
                frame.Dispose();
            }
            return result;
        }
    }

    protected override void DisposeManaged()
    {
        Graph.CloseInputStream("input_video");
        Graph.WaitUntilDone();
        Graph.Dispose();
        observeStreamHandle?.Free();
    }
}
