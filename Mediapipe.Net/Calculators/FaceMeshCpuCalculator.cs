// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Calculators;

public sealed class FaceMeshCpuCalculator : CpuCalculator<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>>
{
	public FaceMeshCpuCalculator()
		: base("mediapipe/graphs/face_mesh/face_mesh_desktop_live.pbtxt", "multi_face_landmarks")
	{
	}
}
