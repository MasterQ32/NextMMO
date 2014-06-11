﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextMMO
{
	/// <summary>
	/// Provides loading and caching of resources.
	/// </summary>
	public abstract class ResourceManager<T>
	{
		private readonly Dictionary<string, T> resources = new Dictionary<string,T>();

		protected ResourceManager()
		{

		}

		protected abstract T Load(string name);

		/// <summary>
		/// Registers an already loaded resource.
		/// </summary>
		/// <param name="name">Name of the resource.</param>
		/// <param name="value">Value of the resource.</param>
		public void Register(string name, T value)
		{
			if (this.resources.ContainsKey(name))
				throw new InvalidOperationException("The resource " + name + " already exists!");
			this.resources.Add(name, value);
		}

		/// <summary>
		/// Loads the resource with the given name.
		/// </summary>
		/// <param name="name">Name of the resource.</param>
		/// <returns>Loaded resource</returns>
		public T this[string name]
		{
			get
			{
				lock (this.resources)
				{
					if (this.resources.ContainsKey(name))
						return this.resources[name];

					var resource = this.Load(name);

					this.resources.Add(name, resource);

					return resource;
				}
			}
		}
	}

	public sealed class FileSystemResourceManager<T> : ResourceManager<T>
	{
		private readonly ResourceLoaderDelegate<T> loader;
		private readonly ResourceSaverDelegate<T> saver;
		private readonly string root;
		private readonly string[] extensions;
		private readonly string defaultExtension;

		/// <summary>
		/// Creates a new resource manager.
		/// </summary>
		/// <param name="root">Root directory.</param>
		/// <param name="loader">Loader delegate to load the resource.</param>
		/// <param name="extensions">Valid file extensions.</param>
		public FileSystemResourceManager(string root, ResourceLoaderDelegate<T> loader, ResourceSaverDelegate<T> saver, params string[] extensions)
		{
			this.root = root;
			this.loader = loader;
			this.saver = saver;
			this.extensions = extensions;
			if(this.extensions.Length > 0)
			{
				this.defaultExtension = this.extensions[0];
			}
			else
			{
				throw new ArgumentOutOfRangeException("extensions", "extensions must have at least one entry");
			}
			if (!Directory.Exists(this.root))
				Directory.CreateDirectory(this.root);
		}

		public void Save(string name, T resource)
		{
			if (this.saver == null)
				throw new InvalidOperationException("Saving not supported.");

			var fileName = this.root + "/" + name + this.defaultExtension;
			using (var stream = File.Open(fileName, FileMode.Create))
			{
				this.saver(stream, resource);
			}
		}

		protected override T Load(string name)
		{
			foreach (var ext in this.extensions)
			{
				var fileName = this.root + "/" + name + ext;
				if (!File.Exists(fileName))
					continue;
				using (var stream = File.Open(fileName, FileMode.Open))
				{
					return this.loader(stream);
				}
			}
			throw new FileNotFoundException(name);
		}
	}
}
