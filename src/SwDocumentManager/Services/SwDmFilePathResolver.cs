// -*- coding: utf-8 -*-
// src/SwDocumentManager/Services/SwDmFilePathResolver.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 解析文档引用路径的服务实现，模拟 SOLIDWORKS 查找被引用文档的搜索规则，支持相对路径递归搜索。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.XCad.Documents.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Services
{
    /// <summary>
    /// Service which provides custom path resolvers
    /// 提供引用文件路径解析能力的服务接口。
    /// </summary>
    public interface IFilePathResolver 
    {
        /// <summary>
        /// Resolves the path of the document
        /// 解析被引用文档的实际路径。
        /// </summary>
        /// <param name="parentDocPath">Parent document full path / 父文档完整路径</param>
        /// <param name="path">Cached document path / 缓存的引用路径</param>
        /// <returns>Resolved path / 解析后的路径</returns>
        string ResolvePath(string parentDocPath, string path);
    }

    /// <summary>
    /// Resolves the reference path for the document
    /// 解析文档引用路径，模拟 SOLIDWORKS 查找被引用文档的搜索规则。
    /// </summary>
    /// <remarks>This logic implemented according to <see href="https://help.solidworks.com/2016/english/SolidWorks/sldworks/c_Search_Routine_for_Referenced_Documents.htm"/>
    /// 该逻辑按照 SOLIDWORKS 官方“引用文档搜索顺序”实现。</remarks>
    public class SwDmFilePathResolver : IFilePathResolver
    {
        /// <summary>
        /// Resolves a component/document reference first relative to the parent document, then falls back to the cached absolute path.
        /// 优先基于父文档目录解析组件或文档引用，失败后再回退到缓存的绝对路径。
        /// </summary>
        public string ResolvePath(string parentDocDirPath, string path)
        {
            string resolvedPath;
            
            if (TrySearchRecursively(parentDocDirPath, path, out resolvedPath))
            {
                return resolvedPath;
            }

            if (IsReferenceExists(path))
            {
                return path;
            }

            throw new FilePathResolveFailedException(path);
        }

        /// <summary>
        /// Searches for the referenced file by walking up parent folders and replaying the relative subfolder structure.
        /// 通过向上回溯父目录并重建相对子目录结构的方式搜索引用文件。
        /// </summary>
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

        /// <summary>
        /// Indicates whether the candidate reference path actually exists on disk.
        /// 判断候选引用路径在磁盘上是否真实存在。
        /// </summary>
        protected virtual bool IsReferenceExists(string path)
            => File.Exists(path);
    }
}
