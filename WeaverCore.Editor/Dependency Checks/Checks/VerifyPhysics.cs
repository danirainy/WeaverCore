﻿using System;
using System.IO;
using UnityEngine;

namespace WeaverCore.Editor
{

    class VerifyPhysics : DependencyCheck
    {
        public override void StartCheck(Action<DependencyCheckResult> finishCheck)
        {
            Physics2D.gravity = new Vector2(0f, -60f);
            finishCheck(DependencyCheckResult.Complete);
        }
    }
}
