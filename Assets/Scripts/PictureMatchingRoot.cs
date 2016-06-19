using UnityEngine;
using System;
using strange.extensions.context.impl;

public class PictureMatchingRoot : ContextView {
	void Awake()
	{
		context = new PictureMatchingContext(this);
	}
}
