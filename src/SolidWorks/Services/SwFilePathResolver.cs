//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.SolidWorks.Documents;
using System.Linq;
using SolidWorks.Interop.swconst;
using Xarial.XCad.SolidWorks.Exceptions;
using Xarial.XCad.Documents.Exceptions;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// Resolves the reference path for the document
    /// <para>中文：解析文档引用文件路径（零件/装配体外部引用）</para>
    /// </summary>
    /// <remarks>This logic implemented according to <see href="https://help.solidworks.com/2016/english/SolidWorks/sldworks/c_Search_Routine_for_Referenced_Documents.htm"/></remarks>
    public abstract class SwFilePathResolverBase : IFilePathResolver
    {
        public string ResolvePath(string parentDocPath, string path)
        {
            // 1) 先检查是否已加载同名文档
            if (TryGetLoadedDocumentPath(path, out string loadedPath)) 
            {
                return loadedPath;
            }

            string resolvedPath;

            foreach (var searchFolder in GetSearchFolders()) 
            {
                // 2) 按 SolidWorks 搜索文件夹规则递归查找
                if (TrySearchRecursively(searchFolder, path, out resolvedPath)) 
                {
                    return resolvedPath;
                }
            }

            // 3) 在父文档目录及其父层级中递归查找
            if (TrySearchRecursively(Path.GetDirectoryName(parentDocPath), path, out resolvedPath))
            {
                return resolvedPath;
            }

            // 4) 最后尝试原始路径
            if (IsReferenceExists(path)) 
            {
                return path;
            }

            throw new FilePathResolveFailedException(path);
        }

        private bool TrySearchRecursively(string targetDirPath, string searchPath, out string resultPath) 
        {
            var targetDir = new DirectoryInfo(targetDirPath);
            var searchDir = new DirectoryInfo(Path.GetDirectoryName(searchPath));

            var fileName = Path.GetFileName(searchPath);

            var pathToCheck = Path.Combine(targetDirPath, fileName);

            if (IsReferenceExists(pathToCheck))
            {
                resultPath = pathToCheck;
                return true;
            }

            var parentDir = targetDir;

            while (parentDir != null)
            {
                var compSubDir = new DirectoryInfo(searchDir.FullName);

                var curSubPath = "";

                while (compSubDir.Parent != null)
                {
                    curSubPath = Path.Combine(compSubDir.Name, curSubPath);

                    pathToCheck = Path.Combine(parentDir.FullName, curSubPath, fileName);

                    if (IsReferenceExists(pathToCheck))
                    {
                        resultPath = pathToCheck;
                        return true;
                    }

                    compSubDir = compSubDir.Parent;
                }

                parentDir = parentDir.Parent;
            }

            resultPath = "";
            return false;
        }

        protected abstract bool TryGetLoadedDocumentPath(string path, out string loadedPath);
        protected abstract string[] GetSearchFolders();
        protected abstract bool IsReferenceExists(string path);
    }

    public class SwFilePathResolver : SwFilePathResolverBase
    {
        private readonly ISwApplication m_App;

        public SwFilePathResolver(ISwApplication app)
        {
            m_App = app;
        }

        protected override bool TryGetLoadedDocumentPath(string path, out string loadedPath)
        {
            var title = Path.GetFileNameWithoutExtension(path);

            var doc = m_App.Documents.FirstOrDefault(d => string.Equals(Path.GetFileNameWithoutExtension(d.Title),
                title, StringComparison.CurrentCultureIgnoreCase));

            if (doc != null)
            {
                loadedPath = doc.Path;
                return true;
            }
            else 
            {
                loadedPath = "";
                return false;
            }
        }

        protected override string[] GetSearchFolders()
        {
            var searchFolders = new string[0];

            var useSearchRule = m_App.Sw.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swUseFolderSearchRules);

            if (useSearchRule)
            {
                var searchFoldersStr = m_App.Sw.GetSearchFolders((int)swSearchFolderTypes_e.swDocumentType);

                if (!string.IsNullOrEmpty(searchFoldersStr))
                {
                    searchFolders = searchFoldersStr.Split(';');
                }
            }

            return searchFolders;
        }

        protected override bool IsReferenceExists(string path) => File.Exists(path);
    }

    public class SwFilePathResolverNoSearchFolders : SwFilePathResolver
    {
        public SwFilePathResolverNoSearchFolders(ISwApplication app) : base(app)
        {
        }

        protected override string[] GetSearchFolders() => new string[0];
    }
}
