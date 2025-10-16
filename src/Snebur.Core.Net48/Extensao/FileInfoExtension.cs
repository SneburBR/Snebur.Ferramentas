using System.Linq;

namespace System.IO
{
    public static class FileInfoExtension
    {
        /// <summary>
        /// Retrieves the penultimate extension of the file if it exists. 
        /// If there is only one extension, it returns that extension. 
        /// If there are no extensions, it returns an empty string.
        /// </summary>
        /// <param name="arquivo">The file information object.</param>
        /// <returns>
        /// The penultimate extension of the file if there are multiple extensions.
        /// The single extension if there is only one.
        /// An empty string if there are no extensions.
        /// </returns>
        /// <example>
        /// For a file named "example.g.cs", this method returns ".g.cs".
        /// For a file named "page.aspx.cs", this method returns ".aspx.cs".
        /// For a file named "index.shtml.ts", this method returns ".shtml.ts".
        /// For a file named "document.txt", this method returns ".txt".
        /// For a file named "file", this method returns an empty string.
        /// </example>
        public static string GetPenultimateExtension(this FileInfo arquivo)
        {
            var countDot = arquivo.Name.Count(c => c == '.');
            if (countDot >= 2)
            {
                var indexPenultimateDot = arquivo.Name.LastIndexOf('.', arquivo.Name.LastIndexOf('.') - 1);
                return arquivo.Name.Substring(indexPenultimateDot);
            }

            if (countDot == 1)
            {
                return arquivo.Extension;
            }
            return String.Empty;
        }
    }
} 