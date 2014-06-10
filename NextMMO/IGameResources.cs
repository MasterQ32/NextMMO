﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NextMMO
{
	public interface IGameResources
	{
		/// <summary>
		/// Provides bitmap resources.
		/// </summary>
		ResourceManager<Bitmap> Bitmaps { get; }

		/// <summary>
		/// Provides animated character sprites.
		/// </summary>
		ResourceManager<AnimatedBitmap> Characters { get; }

		/// <summary>
		/// Provides animated bitmaps.
		/// </summary>
		ResourceManager<AnimatedBitmap> Animations { get; }

		/// <summary>
		/// Provides sounds.
		/// </summary>
		ResourceManager<Sound> Sounds { get; }

		/// <summary>
		/// Provides tile maps.
		/// </summary>
		ResourceManager<TileMap> Maps { get; }
	}
}
