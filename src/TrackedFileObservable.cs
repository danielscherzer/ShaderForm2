﻿using System;
using System.IO;
using System.Reactive.Linq;

namespace ShaderForm2
{
	internal class TrackedFileObservable
	{
		internal static IObservable<string> DelayedLoad(string fileName)
		{
			return CreateFileChangeSequence(fileName)
				.Throttle(TimeSpan.FromSeconds(0.1f))
				.Delay(TimeSpan.FromSeconds(0.1f));
		}

		private static IObservable<string> CreateFileChangeSequence(string fileName)
		{
			var fullPath = Path.GetFullPath(fileName);
			return Observable.Return(fileName).Concat(
				Observable.Using(
				() => new FileSystemWatcher(Path.GetDirectoryName(fullPath) ?? fullPath, Path.GetFileName(fullPath))
				{
					NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.FileName,
					EnableRaisingEvents = true,
				},
				watcher =>
				{
					var fileChanged = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Changed += h, h => watcher.Changed -= h).Select(x => fullPath);
					var fileCreated = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Created += h, h => watcher.Created -= h).Select(x => fullPath);
					var fileRenamed = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => watcher.Renamed += h, h => watcher.Renamed -= h).Select(x => fullPath);
					return fileChanged.Merge(fileCreated).Merge(fileRenamed);
				})
				);
		}
	}
}
