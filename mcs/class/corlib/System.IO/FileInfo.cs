//------------------------------------------------------------------------------
// 
// System.IO.FileInfo.cs 
//
// Copyright (C) 2001 Moonlight Enterprises, All Rights Reserved
// 
// Author:         Jim Richardson, develop@wtfo-guru.com
//                 Dan Lewis (dihlewis@yahoo.co.uk)
// Created:        Monday, August 13, 2001 
//
//------------------------------------------------------------------------------

using System;

namespace System.IO {

	public sealed class FileInfo : FileSystemInfo {
	
		public FileInfo (string path) {
			CheckPath (path);
		
			OriginalPath = path;
			FullPath = Path.GetFullPath (path);
		}

		// public properties

		public override bool Exists {
			get {
				Refresh (false);

				if (stat.Attributes == MonoIO.InvalidFileAttributes)
					return false;

				if ((stat.Attributes & FileAttributes.Directory) != 0)
					return false;

				return true;
			}
		}

		public override string Name {
			get {
				return Path.GetFileName (FullPath);
			}
		}

		public long Length {
			get {
				if (!Exists)
					throw new FileNotFoundException ("Could not find file \"" + OriginalPath + "\".");

				return stat.Length;
			}
		}

		public string DirectoryName {
			get {
				return Path.GetDirectoryName (FullPath);
			}
		}

		public DirectoryInfo Directory {
			get {
				return new DirectoryInfo (DirectoryName);
			}
		}

		// streamreader methods

		public StreamReader OpenText () {
			return new StreamReader (Open (FileMode.Open, FileAccess.Read));
		}

		public StreamWriter CreateText () {
			return new StreamWriter (Open (FileMode.Create, FileAccess.Write));
		}
		
		public StreamWriter AppendText () {
			return new StreamWriter (Open (FileMode.Append, FileAccess.Write));
		}

		// filestream methods
		
		public FileStream OpenRead () {
			return Open (FileMode.Open, FileAccess.Read);
		}

		public FileStream OpenWrite () {
			return Open (FileMode.OpenOrCreate, FileAccess.Write);
		}

		public FileStream Open (FileMode mode) {
			return Open (mode, FileAccess.ReadWrite);
		}

		public FileStream Open (FileMode mode, FileAccess access) {
			return Open (mode, access, FileShare.None);
		}

		public FileStream Open (FileMode mode, FileAccess access, FileShare share) {
			return new FileStream (FullPath, mode, access, share);
		}

		// file methods

		public override void Delete () {
			if (!MonoIO.Exists (FullPath))		// a weird MS.NET behaviour
				return;
		
			if (!MonoIO.DeleteFile (FullPath))
				throw MonoIO.GetException (OriginalPath);
		}
		
		public void MoveTo (string dest) {
			File.Move (FullPath, dest);
		}

		public FileInfo CopyTo (string path) {
			return CopyTo (path, false);
		}

		public FileInfo CopyTo (string path, bool overwrite) {
			string dest = Path.GetFullPath (path);
			File.Copy (FullPath, dest);
		
			return new FileInfo (dest);
		}

		public override string ToString () {
			return OriginalPath;
		}
	}
}
