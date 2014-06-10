﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NextMMO
{
	public interface IEntity
	{
		void Update();

		void Draw(IGraphics graphics, float deltaX, float deltaY);

		double X { get; }

		double Y { get; }

		Sprite Sprite { get; }
	}

	public class IEntityComparer : IComparer<IEntity>
	{
		/// <summary>
		/// Compares two entities.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public int Compare(IEntity a, IEntity b)
		{
			return Math.Sign(a.Y - b.Y);
		}
	}

	public class Entity : IEntity
	{
		private readonly World world;
		private float x;
		private float y;

		public Entity(World world)
		{
			this.world = world;
			this.Size = 0.7; // Make entity 0.5 tiles large.
		}

		public Entity(World world, int x, int y)
			: this(world)
		{
			this.x = x;
			this.y = y;
		}

		public virtual void Update()
		{

		}

		public virtual void Draw(IGraphics graphics, float deltaX, float deltaY)
		{
			this.Sprite.Draw(
				graphics,
				(int)(32 * this.x - deltaX),
				(int)(32 * this.y - deltaY));
		}

		public void Translate(float deltaX, float deltaY)
		{
			var map = this.world.TileMap;

			int cx = (int)(this.x + 0.5);
			int cy = (int)(this.y + 0.5);
			int size = (int)(32 * this.Size);

			// Build "environment" of collision rectangles
			List<Rectangle> environment = new List<Rectangle>();

			// Add "static" environment (contains world boundaries)
			environment.Add(new Rectangle(0, 0, 2, 32 * this.world.TileMap.Height));
			environment.Add(new Rectangle(32 * this.world.TileMap.Width - 2, 0, 2, 32 * this.world.TileMap.Height));

			environment.Add(new Rectangle(0, 0, 32 * this.world.TileMap.Width, 2));
			environment.Add(new Rectangle(0, 32 * this.world.TileMap.Height - 2, 32 * this.world.TileMap.Width, 2));

			// Add "dynamic" environment (contains tile information around the player)
			for (int layer = 0; layer < 2; layer++) // Use only the lower two layers
			{
				for (int px = Math.Max(0, cx - 1); px < Math.Min(map.Width, cx + 2); px++)
				{
					for (int py = Math.Max(0, cy - 1); py < Math.Min(map.Height, cy + 2); py++)
					{
						environment.AddRange(this.world.TileSet[this.world.TileMap[px, py][layer]].CreateEnvironment(px, py));
					}
				}
			}

			// Debug environment
			//foreach (var rect in environment)
			//{
			//	this.world.Debug(rect, Color.Lime);
			//}

			Func<int, int, bool> testCollision = (_x, _y) =>
				{
					Rectangle entity = new Rectangle(
						_x - size / 2,
						_y - size / 2,
						size,
						size);

					// Debug entity collider
					//this.world.Debug(entity, Color.Magenta);

					foreach (var rect in environment)
					{
						if (rect.IntersectsWith(entity))
							return true; // Cancel translation, we will intersect with a wall
					}
					return false;
				};

			var newX = this.x + deltaX;
			var newY = this.y + deltaY;

			if (testCollision((int)(32 * newX + 16), (int)(32 * this.y + 16)))
				deltaX = 0; // Elimitate x movement
			if (testCollision((int)(32 * this.X + 16), (int)(32 * newY + 16)))
				deltaY = 0; // Elimitate x movement

			this.x += deltaX;
			this.y += deltaY;
		}

		public double X { get { return x; } }

		public double Y { get { return y; } }

		public double Size { get; set; }

		public Sprite Sprite { get; set; }

		public IGameServices Services { get { return this.world.Services; } }
	}
}
