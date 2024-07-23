using System;

namespace Gmbc.Common.IO
{
	/// <summary>
	/// Summary description for FileAccess.
	/// Singleton
	/// </summary>
	public class FileAccess
	{
		private FileAccess(){}



		/// <summary>
		/// Reads content of the file into a string.
		/// Please note: this is a function uses System.IO.File.Open() and 
		/// System.IO.StreamReader.ReadToEnd() to do its job - any IOExceptions raised by those functions are not
		/// handled in any way and are simply propagated to the caller. 
		/// </summary>
		/// <param name="filePath">Full path and name of a file to read</param>
		/// <returns>contents of the file as string</returns>
		public static string getFileString(string filePath)
		{
			string sFile = null;
			System.IO.FileStream file = null;
			try
			{
				file = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				sFile = (new System.IO.StreamReader(file)).ReadToEnd();
				file.Close();
			}
			catch (System.IO.IOException e)
			{
				throw e; 
			}
			finally 
			{
				if(file != null) file.Close();
			}
			return sFile;
		}

		/// <summary>
		/// Writes the passed string into the specified file
		/// 
		/// Please note: this is a function uses System.IO.File.Open() and 
		/// System.IO.StreamWriter.Write() to do its job - any IOExceptions raised by those functions are not
		/// handled in any way and are simply propagated to the caller. See documentation for those function
		/// for more information
		/// </summary>
		/// <param name="filePath">Full path and name of a file to write to</param>
		/// <param name="content">string to write to the file</param>
		/// <returns>boolean flag true if done</returns>
		public static bool writeTextFile(string filePath, string content)
		{
			bool bDone = false;
			System.IO.FileStream file = null;
			try 
			{
				file = System.IO.File.Open(filePath, System.IO.FileMode.Create);
				System.IO.StreamWriter sw = new System.IO.StreamWriter(file);
				sw.Write(content);
				sw.Close();
				file.Close(); 
				bDone = true;
			}
			catch (System.IO.IOException e)
			{
				throw e; 
			} 
			finally 
			{
				if(file != null) file.Close();
			}
			return bDone;

		}

		/// <summary>
		/// Writes the content of passed XmlDocument into the specified formatted XML file
		/// </summary>
		/// <param name="filePath">Full path and name of a file to write to</param>
		/// <param name="content">XmlDocument object to write to the file</param>
		/// <returns>boolean flag true if done</returns>
		public static bool writeXMLFile(string filePath, System.Xml.XmlDocument content)
		{
			bool bDone = false;
			System.Xml.XmlTextWriter writer = null;
			try 
			{
				writer = new System.Xml.XmlTextWriter(filePath, null);
				writer.Formatting = System.Xml.Formatting.Indented;
				content.Save(writer);
				bDone = true;
			}
			catch (Exception e)
			{
				throw e; 
			} 
			finally 
			{
				if(writer != null) writer.Close();
			}
			return bDone;

		}

		/// <summary>
		/// Reads content of the directory without filtering.
		/// </summary>
		/// <param name="dirPath">Full path and name of a directory to read content</param>
		/// <returns>contents of the directory as string array</returns>
		public static string[] getDirectoryContents(string dirPath)
		{
			String[] strArray = getDirectoryContents(dirPath, string.Empty);
			return strArray;
		}		
	
		/// <summary>
		/// Reads content of the directory with filtering by type of file.
		/// using file extension ( xml, txt, xsd )
		/// Please note: this is a function uses System.IO.DirectoryInfo() and 
		/// System.IO.FileInfo() to do its job - any IOExceptions raised by those functions are not
		/// handled in any way and are simply propagated to the caller. See documentation for those function
		/// for more information
		/// </summary>
		/// <param name="dirPath">Full path and name of a directory to read content</param>
		/// <param name="fileType">Type filter for files in directory, 
		/// if fileType equals empty string return all files</param>
		/// <returns>contents of the directory as string array</returns>
		public static string[] getDirectoryContents(string dirPath, string fileType)
		{
			String[] strArray;
			try
			{			
				System.Collections.Specialized.StringCollection strCol = new System.Collections.Specialized.StringCollection();
				bool bDoFiltering = false;
				if(!fileType.Equals(string.Empty)) bDoFiltering = true;
				string strFileExt = "."+fileType;
				// Create a reference to the current directory.
				if(dirPath == null) dirPath = Environment.CurrentDirectory;
				System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);
				// Create an array representing the files in the current directory.
				System.IO.FileInfo[] fi = di.GetFiles();
				// Print out the names of the files in the current directory.
				foreach (System.IO.FileInfo fiTemp in fi)
				{
					if(!bDoFiltering) strCol.Add( fiTemp.Name );
					else if(fiTemp.Extension.Equals(strFileExt))
						strCol.Add( fiTemp.Name );
				}
				// Copies the collection to a new array starting at index 0.
				strArray = new String[strCol.Count];
				strCol.CopyTo( strArray, 0 );
			}
			catch (System.IO.IOException e)
			{
				throw e; 
			} 
			finally 
			{
			}
			return strArray;
		}

		/// <summary>
		/// Reads content of the file into a Byte Array.
		/// </summary>
		/// <param name="filePath">Full path and name of a file to read</param>
		/// <returns>contents of the file as byte array</returns>
		public static byte[] getFileBytes(string filePath)
		{
			byte[] bArray;
			System.IO.FileStream fs = null;
			try
			{			
				fs = System.IO.File.OpenRead(filePath); 
				bArray = new byte[fs.Length];
				fs.Read(bArray, 0, (int)fs.Length);
				fs.Close();
			}
			catch (System.IO.IOException e)
			{
				throw e; 
			} 
			finally 
			{
				if(fs != null) fs.Close();
			}
			return bArray;
		}

		/// <summary>
		/// Writes the passed Byte Array into the specified file
		/// 
		/// Please note: this is a function uses System.IO.File.Open() and 
		/// System.IO.BinaryWriter.Write() to do its job - any IOExceptions raised by those functions are not
		/// handled in any way and are simply propagated to the caller. 
		/// </summary>
		/// <param name="filePath">Full path and name of a file to write to</param>
		/// <param name="content">Byte Array to write to the file</param>
		public static bool writeBinaryFile(string filePath, byte[] content)
		{
			bool bDone = false;
			System.IO.BinaryWriter bw = null;
			try 
			{
				bw = new System.IO.BinaryWriter(System.IO.File.Open(filePath, System.IO.FileMode.Create));
				bw.Write(content);
				bw.Close();
				bDone = true;
			}
			catch (System.IO.IOException e)
			{
				throw e;
			}
			finally 
			{
				if(bw != null) bw.Close();
			}
			return bDone;	
			
		}

		/// <summary>
		/// Creates directory with specified path.
		/// </summary>
		/// <param name="path">Full path and name of a directory to create</param>
		/// <returns>boolean flag true if done</returns>
		public static bool writeTextFileWithDirectoryCheckAndCreation(string filePath, string content)
		{
			try 
			{ 
				string dirPath = filePath.Substring(0,filePath.LastIndexOf(@"\"));
				bool dirExists = System.IO.Directory.Exists(dirPath);
				// Determine whether the directory exists.
				if (!dirExists) 
				{
					// Try to create the directory.
					System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(dirPath);
					dirExists = true;
				}

				if(dirExists) writeTextFile(filePath, content);
			} 
			catch (System.IO.IOException e) 
			{
				throw e;
			} 
			finally {}
			return true;
		}

		/// <summary>
		/// Creates directory with specified path.
		/// </summary>
		/// <param name="path">Full path and name of a directory to create</param>
		/// <returns>boolean flag true if done</returns>
		public static bool directoryCheckAndCreation(string filePath)
		{
			bool dirExists = false;
			try 
			{ 
				string dirPath = filePath.Substring(0,filePath.LastIndexOf(@"\"));
				dirExists = System.IO.Directory.Exists(dirPath);
				// Determine whether the directory exists.
				if (!dirExists) 
				{
					// Try to create the directory.
					System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(dirPath);
					dirExists = true;
				}

			} 
			catch (System.IO.IOException e) 
			{
				throw e;
			} 
			finally {}
			return dirExists;
		}

	}
}
