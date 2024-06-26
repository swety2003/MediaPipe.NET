// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Calculators;

[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("Android")]
public sealed class HandGpuCalculator : GpuCalculator<NormalizedLandmarkListPacket, NormalizedLandmarkList>
{
	public HandGpuCalculator()
		: base("mediapipe/graphs/hand_tracking/hand_tracking_desktop_live_gpu.pbtxt", "hand_landmarks")
	{
	}
}
