// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Calculators;

[SupportedOSPlatform("Linux")]
[SupportedOSPlatform("Android")]
public sealed class BlazePoseGpuCalculator : GpuCalculator<NormalizedLandmarkListPacket, NormalizedLandmarkList>
{
	public BlazePoseGpuCalculator()
		: base("mediapipe/graphs/pose_tracking/pose_tracking_gpu.pbtxt", "pose_landmarks")
	{
	}
}
