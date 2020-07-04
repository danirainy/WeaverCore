﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using WeaverCore.Editor.Helpers;

namespace WeaverCore.Editor.Routines
{
	public class WaitForCompilation : IEditorWaiter
	{
		public bool KeepWaiting(float dt)
		{
			return EditorApplication.isCompiling;
		}
	}
}
