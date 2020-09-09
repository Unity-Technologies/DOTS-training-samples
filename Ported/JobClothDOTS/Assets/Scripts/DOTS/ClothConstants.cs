<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothConstants
{
    public static readonly float GRATITY = -9.8f;
}
=======
﻿using Unity.Mathematics;

public static class ClothConstants
{
	public static readonly float3 gravity = new float3(0.0f, -9.81f, 0.0f);
	public static readonly float groundHeight = 0.0f;
}
>>>>>>> 95a77a2159970bc36090860db35bedc60094b026
