// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

using Google.Protobuf;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework
{
    public unsafe class CalculatorGraph : MpResourceHandle
    {
        public delegate Status.StatusArgs NativePacketCallback(IntPtr graphPtr, int streamId, IntPtr packetPtr);
        public delegate void PacketCallback<TPacket, TValue>(TPacket packet) where TPacket : Packet<TValue>;

        public CalculatorGraph() : base()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__(out var ptr).Assert();
            Ptr = ptr;
        }

        private CalculatorGraph(byte[] serializedConfig) : base()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__PKc_i(serializedConfig, serializedConfig.Length, out var ptr).Assert();
            Ptr = ptr;
        }

        public CalculatorGraph(CalculatorGraphConfig config) : this(config.ToByteArray()) { }

        public CalculatorGraph(string textFormatConfig) : this(CalculatorGraphConfig.Parser.ParseFromTextFormat(textFormatConfig)) { }

        protected override void DeleteMpPtr()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__delete(Ptr);
        }

        public Status Initialize(CalculatorGraphConfig config)
        {
            var bytes = config.ToByteArray();
            UnsafeNativeMethods.mp_CalculatorGraph__Initialize__PKc_i(MpPtr, bytes, bytes.Length, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status Initialize(CalculatorGraphConfig config, SidePacket sidePacket)
        {
            var bytes = config.ToByteArray();
            UnsafeNativeMethods.mp_CalculatorGraph__Initialize__PKc_i_Rsp(MpPtr, bytes, bytes.Length, sidePacket.MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        /// <remarks>Crashes if config is not set</remarks>
        public CalculatorGraphConfig Config()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__Config(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var config = serializedProto.Deserialize(CalculatorGraphConfig.Parser);
            serializedProto.Dispose();

            return config;
        }

        public Status ObserveOutputStream(string streamName, int streamId, NativePacketCallback nativePacketCallback, bool observeTimestampBounds = false)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__ObserveOutputStream__PKc_PF_b(MpPtr, streamName, streamId, nativePacketCallback, observeTimestampBounds, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ObserveOutputStream<TPacket, TValue>(string streamName, PacketCallback<TPacket, TValue> packetCallback, bool observeTimestampBounds, out GCHandle callbackHandle) where TPacket : Packet<TValue>, new()
        {
            NativePacketCallback nativePacketCallback = (IntPtr graphPtr, int streamId, IntPtr packetPtr) =>
            {
                try
                {
#pragma warning disable CS8618, 8604
                    var packet = Packet<TValue>.Create<TPacket>(packetPtr, false);
                    packetCallback(packet);
#pragma warning restore CS8618, 8604
                    return Status.StatusArgs.Ok();
                }
                catch (Exception e)
                {
                    return Status.StatusArgs.Internal(e.ToString());
                }
            };
            callbackHandle = GCHandle.Alloc(nativePacketCallback, GCHandleType.Normal);

            return ObserveOutputStream(streamName, 0, nativePacketCallback, observeTimestampBounds);
        }

        public Status ObserveOutputStream<TPacket, TValue>(string streamName, PacketCallback<TPacket, TValue> packetCallback, out GCHandle callbackHandle) where TPacket : Packet<TValue>, new()
        {
            return ObserveOutputStream(streamName, packetCallback, false, out callbackHandle);
        }

        public StatusOrPoller<T> AddOutputStreamPoller<T>(string streamName, bool observeTimestampBounds = false)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__AddOutputStreamPoller__PKc_b(MpPtr, streamName, observeTimestampBounds, out var statusOrPollerPtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrPoller<T>(statusOrPollerPtr);
        }

        public Status Run()
        {
            return Run(new SidePacket());
        }

        public Status Run(SidePacket sidePacket)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__Run__Rsp(MpPtr, sidePacket.MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(sidePacket);
            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status StartRun()
        {
            return StartRun(new SidePacket());
        }

        public Status StartRun(SidePacket sidePacket)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__StartRun__Rsp(MpPtr, sidePacket.MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(sidePacket);
            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status WaitUntilIdle()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__WaitUntilIdle(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status WaitUntilDone()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__WaitUntilDone(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public bool HasError()
        {
            return SafeNativeMethods.mp_CalculatorGraph__HasError(MpPtr);
        }

        public Status AddPacketToInputStream<T>(string streamName, Packet<T> packet)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__AddPacketToInputStream__PKc_Ppacket(MpPtr, streamName, packet.MpPtr, out var statusPtr).Assert();
            packet.Dispose(); // respect move semantics

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status SetInputStreamMaxQueueSize(string streamName, int maxQueueSize)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__SetInputStreamMaxQueueSize__PKc_i(MpPtr, streamName, maxQueueSize, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status CloseInputStream(string streamName)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__CloseInputStream__PKc(MpPtr, streamName, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status CloseAllPacketSources()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__CloseAllPacketSources(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public void Cancel()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__Cancel(MpPtr).Assert();
            GC.KeepAlive(this);
        }

        public bool GraphInputStreamsClosed()
        {
            return SafeNativeMethods.mp_CalculatorGraph__GraphInputStreamsClosed(MpPtr);
        }

        public bool IsNodeThrottled(int nodeId)
        {
            return SafeNativeMethods.mp_CalculatorGraph__IsNodeThrottled__i(MpPtr, nodeId);
        }

        public bool UnthrottleSources()
        {
            return SafeNativeMethods.mp_CalculatorGraph__UnthrottleSources(MpPtr);
        }

        public GpuResources GetGpuResources()
        {
            UnsafeNativeMethods.mp_CalculatorGraph__GetGpuResources(MpPtr, out var gpuResourcesPtr).Assert();

            GC.KeepAlive(this);
            return new GpuResources(gpuResourcesPtr);
        }

        public Status SetGpuResources(GpuResources gpuResources)
        {
            UnsafeNativeMethods.mp_CalculatorGraph__SetGpuResources__SPgpu(MpPtr, gpuResources.SharedPtr, out var statusPtr).Assert();

            GC.KeepAlive(gpuResources);
            GC.KeepAlive(this);
            return new Status(statusPtr);
        }
    }
}
