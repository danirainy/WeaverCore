﻿using System;
using WeaverCore.Editor.Helpers;

namespace WeaverCore.Editor.Routines
{
	public class WaitTillTrue : IEditorWaiter
	{
		Func<bool> Predicate;

		public WaitTillTrue(Func<bool> predicate)
		{
			Predicate = predicate;
		}

		public bool KeepWaiting(float dt)
		{
			return !Predicate();
		}
	}
}
