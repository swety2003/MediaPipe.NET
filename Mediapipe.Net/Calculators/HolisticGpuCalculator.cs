// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework.Packets;

namespace Mediapipe.Net.Calculators;

public sealed class HolisticGpuCalculator : CpuCalculator<BoolPacket, bool>
{
	public HolisticGpuCalculator()
		: base("mediapipe/graphs/holistic_tracking/holistic_tracking_gpu.pbtxt", (string?)null)
	{
	}
}
