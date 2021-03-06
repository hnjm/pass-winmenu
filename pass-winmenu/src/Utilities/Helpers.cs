using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using PassWinmenu.Configuration;
using PassWinmenu.Utilities.ExtensionMethods;

namespace PassWinmenu.Utilities
{
	internal static class Helpers
	{
		/// <summary>
		/// Normalises a directory, replacing all AltDirectorySeparatorChars with DirectorySeparatorChars
		/// and stripping any trailing directory separators.
		/// </summary>
		/// <param name="directory">The directory to be normalised.</param>
		/// <returns>The normalised directory.</returns>
		internal static string NormaliseDirectory(string directory)
		{
			var normalised = directory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			var stripped = normalised.TrimEnd(Path.DirectorySeparatorChar);
			return stripped;
		}
		
		/// <summary>
		/// Returns the path of a file relative to a specified root directory.
		/// </summary>
		/// <param name="filespec">The path to the file or directory for which the relative path should be calculated.</param>
		/// <param name="root">The root directory relative to which the relative path should be calculated.</param>
		/// <returns></returns>
		internal static string GetRelativePath(string filespec, string root)
		{
			if(!Path.IsPathRooted(filespec)) throw new ArgumentException("File spec should be absolute", nameof(filespec));
			if(!Path.IsPathRooted(root)) throw new ArgumentException("Root path must be absolute", nameof(root));

			var rootDir = new DirectoryInfo(root);
			// Even if fileDir is pointing to a file, creating a DirectoryInfo is fine,
			// since we're only concerned about comparing their paths, and DirectoryInfo
			// does not actually check if a directory exists at its location.
			var fileDir = new DirectoryInfo(filespec);
			if (!rootDir.IsParentOf(fileDir))
			{
				throw new ArgumentException("File spec should point to a path within the root directory", nameof(filespec));
			}

			var pathUri = new Uri(filespec);

			// The directory URI must end with a directory separator char.
			if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				root += Path.DirectorySeparatorChar;
			}
			var directoryUri = new Uri(root);
			return Uri.UnescapeDataString(directoryUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		/// <summary>
		/// Retrieves an <see cref="Exception"/> representing the last Win32
		/// error.
		/// </summary>
		internal static Exception LastWin32Exception()
		{
			return Marshal.GetExceptionForHR(
				Marshal.GetHRForLastWin32Error()
				);
		}

		/// <summary>
		/// Converts an ARGB hex colour code into a Color object.
		/// </summary>
		/// <param name="str">A hexadecimal colour code string (such as #AAFF00FF)</param>
		/// <returns>A colour object created from the colour code.</returns>
		internal static Color ColourFromString(string str)
		{
			return (Color)ColorConverter.ConvertFromString(str);
		}

		/// <summary>
		/// Converts an ARGB hex colour code into a SolidColorBrush object.
		/// </summary>
		/// <param name="colour">A hexadecimal colour code string (such as #AAFF00FF)</param>
		/// <returns>A Brush created from a Colour object created from the colour code.</returns>
		internal static Brush BrushFromColourString(string colour)
		{
			if (colour == "[accent]")
			{
				return SystemParameters.WindowGlassBrush;
			}
			return new SolidColorBrush(ColourFromString(colour));
		}

		/// <summary>
		/// Ensures that the thread calling this method is a UI thread.
		/// </summary>
		internal static void AssertOnUiThread()
		{
			var currentThread = Thread.CurrentThread;

			if (currentThread.IsBackground)
			{
				throw new NotOnUiThreadException("Current thread is a background thread.");
			}
			if (currentThread.GetApartmentState() != ApartmentState.STA)
			{
				throw new NotOnUiThreadException("Current thread is not a single-threaded apartment thread.");
			}
			if (currentThread != Application.Current.Dispatcher.Thread)
			{
				throw new NotOnUiThreadException("Current thread is not the current application's dispatcher thread.");
			}
		}

		[Serializable]
		class NotOnUiThreadException : Exception
		{
			public NotOnUiThreadException(string message) : base(message)
			{
			}

			public NotOnUiThreadException(string message, Exception innerException) : base(message, innerException)
			{
			}
		}
	}
}
