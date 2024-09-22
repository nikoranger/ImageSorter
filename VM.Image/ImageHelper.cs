using System.Security.Cryptography;

namespace VM.Image
{
    /// <summary>
    /// 图片信息
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// 创建时间 / 拍摄时间
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 保存MD5的hash值
        /// </summary>
        public string HashCode { get; set; }
    }

    /// <summary>
    /// 帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 读取图片基本信息
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>图片基本信息</returns>
        /// <exception cref="FileNotFoundException"></exception>
        static ImageInfo GetInfo(string path)
        {
            ImageInfo info = new ImageInfo();
            if (File.Exists(path))
            {
                info.Created = File.GetLastWriteTime(path);
                info.Name = Path.GetFileName(path);
                info.Path = path;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        info.HashCode = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
            return info;
        }

        /// <summary>
        /// 将图片拷贝到目标目录
        /// </summary>
        /// <param name="info">图片基本信息</param>
        /// <param name="targetFolder">目标文件夹</param>
        public static void CopyTo(ImageInfo info, string targetFolder)
        {
            var fullpath = Path.Combine(targetFolder, info.Name);
            File.Copy(info.Path, fullpath);
        }

        /// <summary>
        /// 获取目录下所有的图片信息
        /// </summary>
        /// <param name="folder">目标文件夹</param>
        /// <returns>图片文件</returns>
        public static List<ImageInfo> GetInfos(string folder)
        {
            List<ImageInfo> infos = new List<ImageInfo>();
            foreach (var filePath in Directory.GetFiles(folder))
            {
                infos.Add(GetInfo(filePath));
            }
            foreach (var dir in Directory.GetDirectories(folder))
            {
                infos.AddRange(GetInfos(dir));
            }
            return infos;
        }
    }
}
